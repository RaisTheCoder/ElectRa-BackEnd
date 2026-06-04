namespace ElectRa_BackEnd.DataTransferObjects;

public class ProfileDTO
{
	public string Username { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Address { get; set; }
	public string? PhoneNumber { get; set; }
	
	public string? CurrentPassword { get; set; }
	public string? NewPassword { get; set; }
}