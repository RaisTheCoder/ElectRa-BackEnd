using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class SubCategoriesController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public SubCategoriesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
		_context = context;
	}
	
	[HttpGet]
	public async Task<IActionResult> SubCategories(int? limit, int? skip, bool? ignoreDisabled = true)
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

	[HttpGet("[action]/{id}")]
	public async Task<IActionResult> Parent(long id, int? limit, int? skip)
	{
		var query = _context.SubCategories
			.Where(subC => subC.CategoryId == id)
			.AsQueryable();

		if (skip > 0)
			query = query.Skip((int) skip);

		if (limit > 0)
			query = query.Take((int) limit);

		var subCategories = await query.ToListAsync();
		return Ok(subCategories);
	}
	
	[HttpPost("[action]")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Add(SubCategory subCategory, IFormFile? formFile)
	{
		_context.SubCategories.Add(subCategory);
		
		if (formFile != null && formFile.Length > 0)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/categories/sub");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await formFile.CopyToAsync(stream);
			}

			var baseUrl = $"{Request.Scheme}://{Request.Host}"; 
			subCategory.Icon = $"{baseUrl}/uploads/categories/sub/{fileName}";
		}
		
		await _context.SaveChangesAsync();
            
		return Ok();
	}

	[HttpPost("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Update(long id, SubCategory newSubCategory, IFormFile? formFile)
	{
		var subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == id);
		subCategory._Name = newSubCategory._Name;
		subCategory.Slug = newSubCategory.Slug;
		subCategory.CategoryId = newSubCategory.CategoryId;
		
		if (formFile != null && formFile.Length > 0)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/categories/sub");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);
			
			if (!string.IsNullOrEmpty(subCategory.Icon))
			{
				var oldImageName = Path.GetFileName(new Uri(subCategory.Icon).LocalPath);
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
			subCategory.Icon = $"{baseUrl}/uploads/categories/sub/{fileName}";
		}

		await _context.SaveChangesAsync();
		return Ok();
	}
	
	[HttpPut("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Disable(long id, bool? boolean)
	{
		SubCategory? subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == id);

		if (subCategory == null)
			return NotFound();
		
		if (boolean != null)
		{
			subCategory.Enabled = (bool) boolean;
		} else
		{
			subCategory.Enabled = !subCategory.Enabled;
		}
		
		await _context.SaveChangesAsync();
		return Ok();
	}

	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(long id)
	{
		SubCategory? subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == id);
		
		if (subCategory == null)
			return NotFound();

		_context.SubCategories.Remove(subCategory);
		await _context.SaveChangesAsync();
		return Ok();
	}
}