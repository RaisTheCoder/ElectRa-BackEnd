using System.ComponentModel.DataAnnotations;

namespace ElectRa_BackEnd.DataTransferObjects;

public class RegisterDTO
{
	[Required(ErrorMessage = "Cannot leave first name empty.")]
	[StringLength(32, MinimumLength = 3, ErrorMessage = "First Name has to be between 3-32 characters long.")]
	public string firstName { get; set; }
	
	[Required(ErrorMessage = "Cannot leave last name empty.")]
	[StringLength(32, MinimumLength = 3, ErrorMessage = "Last Name has to be between 3-32 characters long.")]
	public string lastName { get; set; }
	
	[Required(ErrorMessage = "Cannot leave username empty.")]
	[StringLength(32, MinimumLength = 3, ErrorMessage = "Last Name has to be between 3-32 characters long.")]
	public string username { get; set; }
	
	[Required(ErrorMessage = "Cannot email address empty.")]
	[DataType(DataType.EmailAddress)]
	public string email { get; set; }
	
	[Phone(ErrorMessage = "Invalid phone number format.")]
	[DataType(DataType.PhoneNumber)]
	public string? phone { get; set; }
	
	[Required(ErrorMessage = "Cannot leave password empty.")]
	[StringLength(64, MinimumLength = 8, ErrorMessage = "Password has to be between 8-64 characters long.")]
	public string password { get; set; }
	
	public string confirmPassword { get; set; }
}