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
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Brands(int? limit, int? skip, bool ignoreDisabled = true)
        {
            var query = _context.Brands.AsQueryable();

            if (ignoreDisabled)
                query = query.Where(b => b.Enabled == true);

            if (skip > 0)
                query = query.Skip((int)skip);

            if (limit > 0)
                query = query.Take((int)limit);

            var brands = await query.ToListAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Brand(long id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Add(Brand brand, IFormFile? formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/brands");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}"; 
                brand.Icon = $"{baseUrl}/uploads/brands/{fileName}";
            }
            _context.Brands.Add(brand);
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update(long id, Brand newBrand, [FromForm] FormFile? formFile)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            brand._Name = newBrand._Name;
            
            if (formFile != null && formFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/brands");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
			
                if (!string.IsNullOrEmpty(brand.Icon))
                {
                    var oldImageName = Path.GetFileName(new Uri(brand.Icon).LocalPath);
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
                brand.Icon = $"{baseUrl}/uploads/brands/{fileName}";
            }

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Disable(long id, bool? boolean)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
            
            if (brand == null)
                return NotFound();

            if (boolean != null)
            {
                brand.Enabled = (bool) boolean;
            } else
            {
                brand.Enabled = !brand.Enabled;
            }
		
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
                return NotFound();
            
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
