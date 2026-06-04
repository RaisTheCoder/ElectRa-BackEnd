namespace ElectRa_BackEnd.Models;

public class VisitHistory
{
	public long Id { get; set; }
	public DateTime Time { get; set; } = DateTime.UtcNow;
	
	public long UserId { get; set; }
	public User? User { get; set; }
	
	public long ProductId { get; set; }
	public Product? Product { get; set; }
}