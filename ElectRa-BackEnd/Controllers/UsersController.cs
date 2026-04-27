using Microsoft.AspNetCore.Mvc;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] User user, [FromForm] IFormFile? formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                user.ProfilePic = "/uploads/" + fileName;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
