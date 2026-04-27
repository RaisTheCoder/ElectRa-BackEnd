using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _context;

	public ProductsController(AppDbContext context)
	{
		_context = context;
	}
	
	[HttpGet]
	public async Task<IActionResult> Products(int? limit, int? skip)
	{
		var query = _context.Products
			.Include(p => p.Category)
			.Include(p => p.Brand)
			.AsQueryable();

		if (skip > 0)
			query = query.Skip((int)skip);

		if (limit > 0)
			query = query.Take((int)limit);

		var products = await query.ToListAsync();

		return Ok(products);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Products(long id)
	{
		Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
		
		if (product == null)
			return NotFound();
		
		return Ok(product);
	}
}