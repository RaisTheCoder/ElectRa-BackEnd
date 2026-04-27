using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class SubCategoriesController : Controller
{
	private readonly AppDbContext _context;

	public SubCategoriesController(AppDbContext context)
	{
		_context = context;
	}
	
	[HttpGet]
	public async Task<IActionResult> SubCategories(int? limit, int? skip)
	{
		var query = _context.SubCategories
			.Include(sc => sc.Category)
			.AsQueryable();

		if (skip > 0)
			query = query.Skip((int) skip);

		if (limit > 0)
			query = query.Take((int) limit);

		var subCategories = await query.ToListAsync();
		
		return Ok(subCategories);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> SubCategory(long id)
	{
		SubCategory? subCategory = await _context.SubCategories.FirstOrDefaultAsync(p => p.Id == id);
		
		if (subCategory == null)
			return NotFound();

		return Ok(subCategory);
	}
}