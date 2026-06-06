using System.Security.Claims;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using ElectRa_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
		_context = context;
	}
	
	[HttpGet]
	public async Task<IActionResult> Products(int? limit, int? skip, string? category, string? subCategory, bool? featured = false, bool? ignoreDisabled = true)
	{
		var query = _context.Products
			.Include(p => p.SubCategory)
			.Include(p => p.Brand)
			.Include(p => p.Reviews)
			.OrderBy(p => p.IsFeatured)
			.Select(p => new 
			{
				p.Id,
				p.Title,
				p.Thumbnail,
				p.Date,
				p.Enabled,
				p.SubCategoryId,
				Price = p.DiscountPercentage > 0
					? p.Price - (p.Price * p.DiscountPercentage.Value / 100m)
					: p.Price,
				OriginalPrice = p.Price,
				p.Stock,
				p.BrandId,
				p.SoldCount,
				p.IsFeatured,
				p.SubCategory,
				p.DiscountPercentage,
				Rating = p.Reviews.Count() > 0
					? Math.Round(p.Reviews.Average(r => r.Rating), 1)
					: 0,
				
				Brand = new
				{
					p.Brand.Id,
					p.Brand._Name
				},

				Reviews = p.Reviews.Select(r => new
				{
					r.Id,
					r.Rating,
					r.Comment,
					reviewHelpfuls = r.reviewHelpfuls.Select(rh => new
					{
						rh.UserId,
						rh.ReviewId
					}),	
					User = new
					{
						r.User.Id,
						r.User.FirstName,
						r.User.LastName,
						r.User.ProfilePic
					}
				})
			})
			// .OrderBy(p => p)
			.AsQueryable();

		if (ignoreDisabled == true)
			query = query.Where(p => p.Enabled == true);

		if (featured == true)
			query = query.Where(p => p.IsFeatured == true);
		
		if (category != null)
			query = query.Where(p => p.SubCategory.Category.Slug == category);

		if (subCategory != null)
			query = query.Where(p => p.SubCategory.Slug == subCategory);
		
		if (skip > 0)
			query = query.Skip((int)skip);

		if (limit > 0)
			query = query.Take((int)limit);

		var products = await query.ToListAsync();

		return Ok(products);
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Products(long id)
	{
		var product = await _context.Products
			.Where(p => p.Id == id)
			.Select(p => new
			{
				p.Id,
				p.Title,
				p.Thumbnail,
				Price = p.DiscountPercentage > 0
					? p.Price - (p.Price * p.DiscountPercentage.Value / 100m)
					: p.Price,
				OriginalPrice = p.Price,
				p.Stock,
				p.Date,
				p.SoldCount,
				p.IsFeatured,
				p.DiscountPercentage,
				Rating = p.Reviews.Count() > 0
					? Math.Round(p.Reviews.Average(r => r.Rating), 1)
					: 0,
				Brand = new
				{
					p.Brand.Id,
					p.Brand._Name
				},

				SubCategory = new
				{
					p.SubCategory.Id,
					p.SubCategory._Name
				},

				Reviews = p.Reviews.Select(r => new
				{
					r.Id,
					r.Rating,
					r.Comment,
					reviewHelpfuls = r.reviewHelpfuls.Select(rh => new
					{
						rh.UserId,
						rh.ReviewId
					}),	
					User = new
					{
						r.User.Id,
						r.User.FirstName,
						r.User.LastName,
						r.User.ProfilePic
					}
				}).ToList()
			})
			.FirstOrDefaultAsync();
		
		if (product == null)
			return NotFound();
		
		return Ok(product);
	}

	[HttpPost("[action]")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Add(Product product, IFormFile? formFile)
	{
		if (formFile != null && formFile.Length > 0)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await formFile.CopyToAsync(stream);
			}
			
			var baseUrl = $"{Request.Scheme}://{Request.Host}";
			product.Thumbnail = $"{baseUrl}/uploads/products/{fileName}";
		}

		product.Date = DateTime.UtcNow;

		_context.Products.Add(product);
		await _context.SaveChangesAsync();
		return Ok(product);
	}
	
	[HttpPost("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Update(long id, [FromForm] Product updated, IFormFile? formFile)
	{
		var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

		if (product == null)
			return NotFound();

		product.Title = updated.Title;
		product.Price = updated.Price;
		product.Stock = updated.Stock;
		product.SubCategoryId = updated.SubCategoryId;
		product.BrandId = updated.BrandId;
		product.IsFeatured = updated.IsFeatured;
		product.DiscountPercentage = updated.DiscountPercentage;

		if (formFile != null && formFile.Length > 0)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);
			
			if (!string.IsNullOrEmpty(product.Thumbnail))
			{
				var oldImageName = Path.GetFileName(new Uri(product.Thumbnail).LocalPath);
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
			product.Thumbnail = $"{baseUrl}/uploads/products/{fileName}";
		}

		await _context.SaveChangesAsync();
		return Ok();
	}
	
	[Authorize]
	[HttpPost("{id}/[action]")]
	public async Task<IActionResult> Favorite(long id)
	{
		var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

		var existing = await _context.ProductFavorites
			.FirstOrDefaultAsync(x => x.ProductId == id && x.UserId == userId);

		if (existing != null)
		{
			_context.ProductFavorites.Remove(existing);
			await _context.SaveChangesAsync();
			return Ok(new { favorite = false });
		}

		_context.ProductFavorites.Add(new Favorite
		{
			ProductId = id,
			UserId = userId
		});

		await _context.SaveChangesAsync();
		return Ok(new { favorite = true });
	}
	
	[HttpPut("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Disable(long id, bool? boolean)
	{
		Product? product = await _context.Products.FirstOrDefaultAsync(u => u.Id == id);

		if (product == null)
			return NotFound();

		if (boolean != null)
		{
			product.Enabled = (bool) boolean;
		} else
		{
			product.Enabled = !product.Enabled;
		}
		
		await _context.SaveChangesAsync();
		return Ok();
	}
	
	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(long id)
	{
		Product? product = await _context.Products.FirstOrDefaultAsync(sc => sc.Id == id);
		
		if (product == null)
			return NotFound();

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();
		return Ok();
	}
}