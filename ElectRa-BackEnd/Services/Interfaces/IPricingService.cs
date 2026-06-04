using ElectRa_BackEnd.Models;

namespace ElectRa_BackEnd.Services.Interfaces;

public interface IPricingService
{
	decimal GetFinalPrice(Product product);
}