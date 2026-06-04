namespace ElectRa_BackEnd.Models;

public class OrderItem
{
	public long Id { get; set; }
	
	public int Quantity { get; set; }
	
	public decimal Price { get; set; }
	public decimal TotalPrice { get; set; }
	
	public string Title { get; set; }
	public long ProductId { get; set; }
	public Product? Product { get; set; }
	
	public long OrderId { get; set; }
	public Order? Order { get; set; }
}