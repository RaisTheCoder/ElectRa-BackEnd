namespace ElectRa_BackEnd.DataTransferObjects;

public class ReviewDTO
{
	public string Comment { get; set; }
	public decimal Rating { get; set; }
	public int ProductId { get; set; }
}