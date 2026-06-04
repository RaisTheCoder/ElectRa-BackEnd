using System.Linq;
using System.Threading.Tasks;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.DataTransferObjects;
using ElectRa_BackEnd.Helpers;
using ElectRa_BackEnd.Models;
using ElectRa_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly UserManager<User> _userManager;
	private readonly IPricingService _pricingService;

	public OrdersController(AppDbContext context, UserManager<User> userManager, IPricingService pricingService)
	{
		_context = context;
		_userManager = userManager;
		_pricingService = pricingService;
	}

	[HttpGet]
	public async Task<IActionResult> Orders(int? limit, int? skip)
	{
		var query = _context.Orders.AsQueryable().Select(o => new
		{
			o.Id,
			o.Address,
			o.Items,
			o.CreatedAt,
			o.Note,
			o.TotalPrice,
			o.Status,
			o.UserId,
			o.User,
			o.AdminNote,
			o.Phone,
			o.TrackingNumber
		});

		if (skip > 0)
			query = query.Skip((int) skip);

		if (limit > 0)
			query = query.Take((int) limit);

		var orders = await query.ToListAsync();

		return Ok(orders);
	}
	
	[HttpGet("{id}")]
	public async Task<IActionResult> Order(long id)
	{
		var order = await _context.Orders.Where(o => o.Id == id).Select(o => new
		{
			o.Id,
			o.Address,
			o.Items,
			o.CreatedAt,
			o.Note,
			o.TotalPrice,
			o.Status,
			o.UserId,
			o.User,
			o.AdminNote,
			o.Phone,
			o.TrackingNumber
		}).FirstOrDefaultAsync();
		
		if (order == null)
			return NotFound();

		return Ok(order);
	}
	
	[Authorize(Roles = "Admin, Manager")]
	[HttpPut("{id}/update")]
	public async Task<IActionResult> Update(long id, UpdateOrderAdminDTO dto)
	{
		var order = await _context.Orders.FindAsync(id);

		if (order == null)
			return NotFound();

		if (dto.Status.HasValue)
			order.Status = dto.Status.Value;

		if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
			order.TrackingNumber = dto.TrackingNumber;

		if (!string.IsNullOrWhiteSpace(dto.AdminNote))
			order.AdminNote = dto.AdminNote;

		await _context.SaveChangesAsync();

		return Ok(new
		{
			order.Id,
			order.Status,
			order.TrackingNumber,
			order.AdminNote
		});
	}

	[Authorize(Roles = "Admin, Manager")]
	[HttpPut("{id}")]
	public async Task<IActionResult> ChangeStatus(long id, OrderStatus status)
	{
		var currentUser = await _userManager.GetUserAsync(User);
		var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

		if (order == null)
			return NotFound();

		order.Status = status;

		if (order.Status == OrderStatus.Delivered && !order.PointsGranted)
		{
			currentUser.RewardPoints += order.EarnedPoints;
			order.PointsGranted = true;
		}

		await _context.SaveChangesAsync();
		return Ok();
	}

	[Authorize]
	[HttpPost("checkout")]
	public async Task<IActionResult> Checkout(CheckoutDTO checkoutDto)
	{
		var currentUser = await _userManager.GetUserAsync(User);

		if (currentUser == null)
			return Unauthorized();

		if (checkoutDto?.Items == null || !checkoutDto.Items.Any())
			return BadRequest("Cart is empty");

		var productIds = checkoutDto.Items.Select(x => x.ProductId).ToList();

		var products = await _context.Products
			.Where(p => productIds.Contains(p.Id))
			.ToListAsync();

		if (products.Count != productIds.Count)
			return BadRequest("Some products are invalid");
		
		var productMap = products.ToDictionary(p => p.Id);

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			var order = new Order
			{
				UserId = currentUser.Id,
				CreatedAt = DateTime.UtcNow,
				Status = OrderStatus.Pending,
				Items = new List<OrderItem>(),
				Address = checkoutDto.Address,
				Phone = checkoutDto.Phone,
				Note = checkoutDto.Note
			};

			decimal total = 0;

			foreach (var item in checkoutDto.Items)
			{
				var product = productMap[item.ProductId];

				if (product == null)
					return BadRequest("Invalid product in cart");

				if ((bool)!product.Enabled)
					return BadRequest($"Product {product.Title} is unavailable");

				if (product.Stock < item.Quantity)
					return BadRequest($"{product.Title} does not have enough stock");

				product.Stock -= item.Quantity;
				product.SoldCount += item.Quantity;

				if (product.Stock <= 0)
					product.Enabled = false;

				var unitPrice = _pricingService.GetFinalPrice(product);
				
				var orderItem = new OrderItem
				{
					ProductId = product.Id,
					Title = product.Title,
					Quantity = item.Quantity,
					Price = unitPrice,
					TotalPrice = unitPrice * item.Quantity
				};

				total += orderItem.TotalPrice;
				
				order.Items.Add(orderItem);
			}
			
			var maxUsablePoints = (int)(total * 0.15m);

			if (checkoutDto.UsedPoints > 0)
			{
				if (currentUser.RewardPoints < 5)
					return BadRequest("Not enough points to use.");

				if (checkoutDto.UsedPoints > currentUser.RewardPoints)
					return BadRequest("You don't have enough points.");

				if (checkoutDto.UsedPoints > maxUsablePoints)
					return BadRequest("You can only use up to 15% of order total.");

				total -= checkoutDto.UsedPoints;
				currentUser.RewardPoints -= checkoutDto.UsedPoints;
			}

			order.TotalPrice = total;
			order.EarnedPoints = (int)(total * 0.3m);
			order.PointsUsed = checkoutDto.UsedPoints;
			order.PointsGranted = false;

			_context.Orders.Add(order);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			if (!string.IsNullOrWhiteSpace(checkoutDto.Address))
			{
				currentUser.Address = checkoutDto.Address;
				await _userManager.UpdateAsync(currentUser);
			}

			if (!string.IsNullOrWhiteSpace(checkoutDto.Phone))
			{
				await _userManager.SetPhoneNumberAsync(currentUser, checkoutDto.Phone);
			}

			return Ok();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}
}