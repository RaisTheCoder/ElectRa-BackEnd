using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.DataTransferObjects;
using ElectRa_BackEnd.Helpers;
using ElectRa_BackEnd.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<IdentityRole<long>> _roleManager;
	private readonly SignInManager<User> _signInManager;
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;

	// Roles
	private string[] roles = Enum.GetNames(typeof(RoleChange.Role));
	
	public AccountController(UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager, SignInManager<User> signInManager, AppDbContext context, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
		_signInManager = signInManager;
		_webHostEnvironment = webHostEnvironment;
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> Me()
	{
		var user = await _userManager.GetUserAsync(User);

		if (user == null)
			return Unauthorized();

		var roles = await _userManager.GetRolesAsync(user);
		var history = await _context.VisitHistories.Where(vH => vH.UserId == user.Id)
			.Select(vH =>
			new {
				vH.Id,
				vH.UserId,
				vH.Product,
				vH.ProductId,
				vH.Time
			})
			.ToListAsync();

		var orders = await _context.Orders.Where(o => o.UserId == user.Id)
			.Select(o =>
				new
				{
					o.Id,
					o.UserId,
					o.Items,
					o.CreatedAt,
					o.Note,
					o.Status,
					o.TotalPrice,
					o.Address
				}).ToListAsync();

		return Ok(new
		{
			id = user.Id,
			username = user.UserName,
			email = user.Email,
			phoneNumber = user.PhoneNumber,
			address = user.Address,
			firstName = user.FirstName,
			lastName = user.LastName,
			profilePic = user.ProfilePic,
			rewardPoints = user.RewardPoints,
			history = history,
			orders = orders,
			roles
		});
	}

	public async Task CreateRole()
	{
		foreach (var role in roles)
		{
			if (!await _roleManager.RoleExistsAsync(role))
			{
				await _roleManager.CreateAsync(new IdentityRole<long>(role));
			}
		}
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> Login(LoginDTO dto)
	{
		var user = await _userManager.FindByNameAsync(dto.Username);
		var roles = await _userManager.GetRolesAsync(user);

		if (!roles.Contains("Admin") && user.LockoutEnabled)
		{ 
			return Unauthorized();
		}
		
		var result = await _signInManager.PasswordSignInAsync(
			dto.Username,
			dto.Password,
			true,
			false
		);

		if (!result.Succeeded)
			return Unauthorized("Invalid credentials");

		return Ok(new
		{
			id = user.Id,
			username = user.UserName,
			roles
		});
	}
	
	[HttpPost("google")]
	public async Task<IActionResult> GoogleLogin([FromBody] GoogleDTO dto)
	{
		var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Token);

		var user = await _userManager.FindByEmailAsync(payload.Email);

		if (user.LockoutEnabled) return Unauthorized();

		if (user == null)
		{
			user = new User
			{
				ProfilePic = payload.Picture,
				UserName = payload.Email,
				Email = payload.Email,
				FirstName = payload.GivenName,
				LastName = payload.FamilyName
			};

			await _userManager.CreateAsync(user);
			await _userManager.AddToRoleAsync(user, "Customer");
		}
		
		await _signInManager.SignInAsync(user, isPersistent: true);

		return Ok(new { message = "Logged in with Google" });
	}
	
	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();

		return Ok();
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDTO user)
	{
		if (_roleManager.Roles.Count() <= roles.Length)
			await CreateRole();

		var newUser = new User
		{
			FirstName = user.firstName,
			LastName = user.lastName,
			UserName = user.username,
			Email = user.email,
			PhoneNumber = user.phone,
		};

		var result = await _userManager.CreateAsync(newUser, user.password);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		if (_userManager.Users.Count() == 1)
		{
			await _userManager.AddToRoleAsync(newUser, "Admin");
		}
		else
		{
			await _userManager.AddToRoleAsync(newUser, "Customer");
		}

		await _signInManager.SignInAsync(newUser, true);

		return Ok();
	}
	
	[Authorize]
	[HttpDelete]
	public async Task<IActionResult> DeleteAccount()
	{
		var user = await _userManager.GetUserAsync(User);
		var baseUrl = $"{Request.Scheme}://{Request.Host}";

		if (user == null)
			return NotFound();
		
		await _userManager.SetUserNameAsync(user, $"deleted_{user.Id}");
		
		user.ProfilePic = $"{baseUrl}/images/deleted-account.png";
		
		await _userManager.SetLockoutEnabledAsync(user, true);
		await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
		
		await _userManager.UpdateAsync(user);
		
		await _signInManager.SignOutAsync();

		return Ok();
	}

	[Authorize]
	[HttpPut("me")]
	public async Task<IActionResult> Update([FromForm] ProfileDTO newProfile, [FromForm] IFormFile? formFile)
	{
		var currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null) return Unauthorized();

		await _userManager.SetUserNameAsync(currentUser, newProfile.Username);
		await _userManager.SetPhoneNumberAsync(currentUser, newProfile.PhoneNumber);
		await _userManager.SetEmailAsync(currentUser, newProfile.Email);
		currentUser.FirstName = newProfile.FirstName;
		currentUser.LastName = newProfile.LastName;
		currentUser.Address = newProfile.Address;
		
		if (formFile != null && formFile.Length > 0)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/users");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);
			
			if (!string.IsNullOrEmpty(currentUser.ProfilePic))
			{
				var oldImageName = Path.GetFileName(new Uri(currentUser.ProfilePic).LocalPath);
				var oldImagePath = Path.Combine(uploadsFolder, oldImageName);

				if (System.IO.File.Exists(oldImagePath))
				{
					System.IO.File.Delete(oldImagePath);
				}
			}
			
			var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using var stream = new FileStream(filePath, FileMode.Create);
			await formFile.CopyToAsync(stream);

			var baseUrl = $"{Request.Scheme}://{Request.Host}";
			currentUser.ProfilePic = $"{baseUrl}/uploads/users/{fileName}";
		}

		if (!string.IsNullOrWhiteSpace(newProfile.CurrentPassword) &&
		    !string.IsNullOrWhiteSpace(newProfile.NewPassword))
		{
			var result = await _userManager.ChangePasswordAsync(
				currentUser,
				newProfile.CurrentPassword,
				newProfile.NewPassword
			);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
		}
		
		await _userManager.UpdateAsync(currentUser);
		await _signInManager.RefreshSignInAsync(currentUser);
		await _context.SaveChangesAsync();
		return Ok();
	}
	
	[Authorize(Roles = "Admin, Manager")]
	[HttpGet("admin/dashboard")]
	public async Task<IActionResult> GetDashboard()
	{
		long? totalSold = 0;
		var productSold = _context.Products.Select(p => p.SoldCount);
		var mostSold = _context.Products
			.OrderByDescending(p => p.SoldCount)
			.Take(5)
			.Select(p => new 
			{ 
				p.Id, 
				p.Title, 
				Rating = p.Reviews.Any()
					? Math.Round(p.Reviews.Average(r => r.Rating), 1)
					: 0,
				p.Thumbnail,
				p.SubCategoryId,
				p.Brand,
				p.SoldCount
		});
		
		foreach (var p in productSold)
		{
			totalSold += p;
		}
		
		return Ok(new {
			users = await _userManager.Users.CountAsync(),
			products = await _context.Products.CountAsync(),
			reviews = await _context.Reviews.CountAsync(),
			categories = await _context.Categories.CountAsync(),
			productsSold = totalSold,
			mostSold = mostSold
		});
	}
}