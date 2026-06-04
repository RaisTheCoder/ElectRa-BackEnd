using ElectRa_BackEnd.Helpers;

namespace ElectRa_BackEnd.Models;

public class Order
{
	public long Id { get; set; }
	public string? TrackingNumber { get; set; }
	
	public string? Note { get; set; }
	public string? AdminNote { get; set; }

	public decimal TotalPrice { get; set; }
	public decimal PointsUsed { get; set; }
	public decimal EarnedPoints { get; set; }
	public bool PointsGranted { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public OrderStatus Status { get; set; } = OrderStatus.Pending;
	public string Address { get; set; }
	public string Phone { get; set; }
	
	public long UserId { get; set; }
	public User? User { get; set; }

	public List<OrderItem> Items { get; set; }
}