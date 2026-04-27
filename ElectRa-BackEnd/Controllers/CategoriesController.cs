using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;

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

        public async Task<IActionResult> Categories(int? limit, int? skip)
        {
            var query = _context.Categories
                .Include(c => c.Products)
                .AsQueryable();

            if (skip > 0)
                query.Skip((int)skip);
            
            if (limit > 0)
                query.Take((int) limit);

            var categories = await query.ToListAsync();
            
            return Ok(categories);
        }
    }
}
