using ElectRa_BackEnd.Models;
using ElectRa_BackEnd.Services.Interfaces;

namespace ElectRa_BackEnd.Services.Implementations;

public class PricingService : IPricingService
{
	public decimal GetFinalPrice(Product product)
	{
		if (product.DiscountPercentage is null or <= 0)
			return product.Price;

		return product.Price - (product.Price * product.DiscountPercentage.Value / 100m);
	}
}