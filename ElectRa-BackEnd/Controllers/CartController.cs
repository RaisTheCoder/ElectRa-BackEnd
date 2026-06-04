using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.DataTransferObjects;
using ElectRa_BackEnd.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectRa_BackEnd.Models;

namespace ElectRa_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] List<CartItemDTO> items)
    {
        var productIds = items.Select(x => x.ProductId).ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Include(p => p.Reviews)
            .ToListAsync();

        var result = new List<ValidatedCartItemDTO>();

        foreach (var item in items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);

            if (product == null || (bool) !product.Enabled)
            {
                result.Add(new ValidatedCartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Title = "Unavailable",
                    Price = 0,
                    Available = false
                });

                continue;
            }

            result.Add(new ValidatedCartItemDTO
            {
                ProductId = product.Id,
                Title = product.Title,
                Thumbnail = product.Thumbnail,
                Price = product.DiscountPercentage > 0
                    ? product.Price - (product.Price * product.DiscountPercentage.Value / 100m)
                    : product.Price,
                OriginalPrice = product.Price,
                DiscountPercentage = product.DiscountPercentage,
                Rating = product.Reviews.Count() > 0
                    ? Math.Round(product.Reviews.Average(r => r.Rating), 1)
                    : 0,
                Quantity = item.Quantity,
                Available = true
            });
        }

        return Ok(result);
    }
}