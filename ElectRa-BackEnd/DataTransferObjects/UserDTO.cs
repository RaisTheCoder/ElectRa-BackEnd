namespace ElectRa_BackEnd.DataTransferObjects;

public class UserDTO
{
	public long Id { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }

	public string FirstName { get; set; }
	public string LastName { get; set; }

	public string? PhoneNumber { get; set; }
	public string? Address { get; set; }

	public List<string> Roles { get; set; }

	public bool LockoutEnabled { get; set; }
}