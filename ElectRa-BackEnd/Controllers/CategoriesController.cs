using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;

namespace ElectRa_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Categories(int? limit, int? skip, bool? ignoreDisabled = true)
        {
            var query = _context.Categories
                .Include(c => c.SubCategories)
                .AsQueryable();

            if (ignoreDisabled == true)
                query = query.Where(c => c.Enabled == true);
            
            if (skip > 0)
                query = query.Skip((int)skip);
            
            if (limit > 0)
                query = query.Take((int) limit);

            var categories = await query.ToListAsync();
            
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Category(long id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
                return NotFound();
            
            return Ok(category);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, Category newCategory)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            category._Name = newCategory._Name;
            category.Slug = newCategory.Slug;

            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable(long id, bool? boolean)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (category == null)
                return NotFound();
            
            if (boolean != null)
            {
                category.Enabled = (bool) boolean;
            } else
            {
                category.Enabled = !category.Enabled;
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(sc => sc.Id == id);
		
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
