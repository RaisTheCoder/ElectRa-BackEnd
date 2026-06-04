using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ElectRa_BackEnd.Models;

[
	Index(nameof(UserName), IsUnique = true),
	Index(nameof(Email), IsUnique = true),
]
public class User : IdentityUser<long>
{
	public string? ProfilePic { get; set; }
	public string FirstName { get; set; } 
	public string? LastName { get; set; }
	public string? Address { get; set; }
	public decimal? RewardPoints { get; set; } = 0m;
	
	public string? LockoutReason { get; set; }
	
	public List<Review>? Reviews { get; set; }
	public List<Favorite> Favorites { get; set; }
	public List<VisitHistory>? VisitHistory { get; set; }
	public List<Order> Orders { get; set; }
	
	[NotMapped]
	public FormFile? FormFile { get; set; }
}