using System.ComponentModel.DataAnnotations;

namespace ElectRa_BackEnd.DataTransferObjects;

public class CheckoutDTO
{
	[Required]
	public List<CartItemDTO> Items { get; set; }
	
	[Required]
	public string Address { get; set; } = string.Empty;
	public string? Phone { get; set; }
	public string? Note { get; set; }
	public decimal UsedPoints { get; set; }
}