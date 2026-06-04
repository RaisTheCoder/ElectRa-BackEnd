namespace ElectRa_BackEnd.Models;

public class Favorite
{
	public long UserId { get; set; }
	public User User { get; set; }

	public long ProductId { get; set; }
	public Product Product { get; set; }
	
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}