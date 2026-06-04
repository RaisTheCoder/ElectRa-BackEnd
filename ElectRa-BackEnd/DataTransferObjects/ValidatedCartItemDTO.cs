namespace ElectRa_BackEnd.DataTransferObjects;

public class ValidatedCartItemDTO
{
	public long ProductId { get; set; }
	public string Title { get; set; }
	public string Thumbnail { get; set; }
	public decimal Price { get; set; }
	public decimal OriginalPrice { get; set; }
	public int? DiscountPercentage { get; set; }
	public decimal Rating { get; set; }
	public int Quantity { get; set; }
	public bool Available { get; set; }
}