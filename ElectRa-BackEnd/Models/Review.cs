using Newtonsoft.Json;

namespace ElectRa_BackEnd.Models;

public class Review
{
	public long Id { get; set; }
	public string Comment { get; set; }
	public decimal Rating { get; set; }
	public int? foundThisHelpful { get; set; }
	
	public long UserId { get; set; }
	public User? User { get; set; }
	
	public long ProductId { get; set; }
	
	public List<ReviewHelpful>? reviewHelpfuls { get; set; }
	
	[JsonIgnore]
	public Product? Product { get; set; }
}