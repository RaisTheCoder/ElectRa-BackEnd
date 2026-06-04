using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.DataTransferObjects;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;

namespace ElectRa_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Reviews(int? limit, int? skip)
        {
            var query = _context.Reviews
                .AsQueryable()
                .Include(r => r.reviewHelpfuls)
                .Include(r => r.User)
                .Select(r => new
                {
                    r.Id,
                    r.Comment,
                    r.UserId,
                    r.Rating,
                    r.ProductId,
                    r.Product,
                    reviewHelpfuls = r.reviewHelpfuls.Select(rh => new
                    {
                        rh.UserId,
                        rh.ReviewId
                    }),
                    r.User
                });

            if (skip < 0)
                query.Skip((int) skip);

            if (limit > 0)
                query.Take((int)limit);

            var reviews = await query.ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Reviews(long id)
        {
            var review = await _context.Reviews
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Comment,
                    r.UserId,
                    r.ProductId,
                    r.Product,
                    reviewHelpfuls = r.reviewHelpfuls.Select(rh => new
                    {
                        rh.UserId,
                        rh.ReviewId
                    })
                })
                .FirstOrDefaultAsync();

            if (review == null)
                return NotFound();
            
            return Ok(review);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(ReviewDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = new Review
            {
                Comment = dto.Comment,
                Rating = dto.Rating,
                ProductId = dto.ProductId,
                UserId = long.Parse(userId),
                foundThisHelpful = 0
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [Authorize]
        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Helpful(long id)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existing = await _context.ReviewHelpfuls
                .FirstOrDefaultAsync(x => x.ReviewId == id && x.UserId == userId);

            if (existing != null)
            {
                _context.ReviewHelpfuls.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { helpful = false });
            }

            _context.ReviewHelpfuls.Add(new ReviewHelpful
            {
                ReviewId = id,
                UserId = userId
            });

            await _context.SaveChangesAsync();
            return Ok(new { helpful = true });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();
            
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
