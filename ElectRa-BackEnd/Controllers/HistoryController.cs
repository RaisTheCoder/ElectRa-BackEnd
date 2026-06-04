using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.DataTransferObjects;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly UserManager<User> _userManager;

	public HistoryController(AppDbContext context, UserManager<User> userManager)
	{
		_userManager = userManager;
		_context = context;
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> History(int? limit, int? skip)
	{
		var currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null) return Unauthorized("User is not logged in or does not exist.");

		var query = _context.VisitHistories.AsQueryable().Where(h => h.UserId == currentUser.Id);

		if (skip > 0)
			query = query.Skip((int)skip);

		if (limit > 0)
			query = query.Take((int)limit);

		var history = await query.ToListAsync();
		
		return Ok(history);
	}

	[HttpPost("push")]
	[Authorize]
	public async Task<IActionResult> Push(long productId)
	{
		var currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null) return Unauthorized();
		
		var newHistory = new VisitHistory
		{
			UserId = currentUser.Id,
			ProductId = productId,
			Time = DateTime.UtcNow
		};
		
		await _context.VisitHistories.AddAsync(newHistory);
		await _context.SaveChangesAsync();
		return Ok();
	}

	[HttpDelete("{id}")]
	[Authorize]
	public async Task<IActionResult> Delete(long id)
	{
		var currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null) return Unauthorized();

		await _context.VisitHistories.Where(h => h.Id == id).ExecuteDeleteAsync();
		return Ok();
	}

	[HttpDelete("clear")]
	public async Task<IActionResult> Clear()
	{
		var currentUser = await _userManager.GetUserAsync(User);

		if (currentUser == null)
			return Unauthorized();

		await _context.VisitHistories
			.Where(h => h.UserId == currentUser.Id)
			.ExecuteDeleteAsync();

		return Ok();
	}
}