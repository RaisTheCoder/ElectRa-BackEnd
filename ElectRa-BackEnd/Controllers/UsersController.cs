using Microsoft.AspNetCore.Mvc;
using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace ElectRa_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;

        public UsersController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Users(int? limit, int? skip)
        {
            var query = _userManager.Users
                .Skip(skip ?? 0)
                .Take(limit ?? 10);

            var users = await query.ToListAsync();

            var result = new List<object>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                result.Add(new
                {
                    u.Id,
                    u.ProfilePic,
                    u.FirstName,
                    u.LastName,
                    u.Address,
                    u.UserName,
                    u.Email,
                    u.AccessFailedCount,
                    u.TwoFactorEnabled,
                    u.EmailConfirmed,
                    u.LockoutEnabled,
                    u.LockoutReason,
                    u.LockoutEnd,

                    Roles = roles,

                    Reviews = (u.Reviews ?? Enumerable.Empty<Review>())
                        .Select(r => new
                        {
                            r.Id,
                            r.Comment,
                            Product = new
                            {
                                r.Product.Id,
                                r.Product.Title
                            }
                        })
                        .ToList(),

                    History = (u.VisitHistory ?? Enumerable.Empty<VisitHistory>())
                        .Select(vH => new
                        {
                            vH.Id,
                            vH.Time,
                            Product = new
                            {
                                vH.Product.Id,
                                vH.Product.Title,
                                vH.Product.Thumbnail,
                                vH.Product.Price,
                                Category = vH.Product.SubCategory != null
                                    ? vH.Product.SubCategory._Name
                                    : null
                            }
                        })
                        .ToList(),

                    Favorites = (u.Favorites ?? Enumerable.Empty<Favorite>())
                        .Select(f => new
                        {
                            f.UserId,
                            f.ProductId,
                            f.CreatedAt
                        })
                        .ToList(),
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> User(long id)
        {
            var user = _context.Users.Where(u => u.Id == id).Select(u => new
            {
                u.Id,
                u.ProfilePic,
                u.FirstName,
                u.LastName,
                u.Address,
                u.UserName,
                u.Email,
                u.AccessFailedCount,
                u.TwoFactorEnabled,
                u.EmailConfirmed,
                u.LockoutEnabled,
                u.LockoutReason,
                u.LockoutEnd,
                Reviews = u.Reviews.Select(r => new 
                {
                    r.Id,
                    r.Comment,
                    Product = new
                    {
                        r.Product.Title,
                        r.Product.Id
                    },
                }),
                History = u.VisitHistory.Select(vH => new
                {
                    vH.Id,
                    vH.Time,
                    Product = new
                    {
                        Id = vH.Product.Id,
                        Title = vH.Product.Title,
                        Thumbnail = vH.Product.Thumbnail,
                        Price = vH.Product.Price,
                        Categoy = vH.Product.SubCategory._Name
                    }
                }),
                Favorites = u.Favorites.Select(f => new
                {
                    f.UserId,
                    f.ProductId,
                    f.CreatedAt
                })
            });

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("change-role/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(long id, string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable(long id, bool? boolean)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (boolean != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, (bool) boolean);
            } else
            {
                await _userManager.SetLockoutEnabledAsync(user, !user.LockoutEnabled);
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
