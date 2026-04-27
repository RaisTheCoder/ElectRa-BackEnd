namespace ElectRa_BackEnd.Models;

public class Review
{
	public long Id { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public decimal Rating { get; set; }
	
	public long UserId { get; set; }
	public User User { get; set; }
	
	public long ProductId { get; set; }
	public Product Product { get; set; }
}