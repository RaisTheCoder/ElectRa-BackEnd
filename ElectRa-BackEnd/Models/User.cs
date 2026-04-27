using System.ComponentModel.DataAnnotations.Schema;

namespace ElectRa_BackEnd.Models;

public class User
{
	public long Id { get; set; }
	
	public string? ProfilePic { get; set; }
	public string FirstName { get; set; } 
	public string LastName { get; set; }
	
	public string Username { get; set; }
	public string Email { get; set; }
	public string? Phone { get; set; }
	public string Address { get; set; }
	
	public string Password { get; set; }
	public bool Enabled { get; set; } = true;
	
	public List<Review>? Reviews { get; set; }
	
	[NotMapped]
	public FormFile? FormFile { get; set; }
}