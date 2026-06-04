using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectRa_BackEnd.Models;

public class Product
{
	public long Id { get; set; }
	public string Title { get; set; }
	public string? Thumbnail { get; set; }
	
	[DefaultValue("true")]
	public bool? Enabled { get; set; } = true;
	
	public long Stock { get; set; }
	public long CategoryId { get; set; }
	public long SubCategoryId { get; set; }
	public SubCategory? SubCategory { get; set; }
	
	public long BrandId { get; set; }
	public Brand? Brand { get; set; }
	public DateTime? Date { get; set; }
	
	public List<Favorite>? Favorites { get; set; }
	
	public long? SoldCount { get; set; }
	
	public decimal Price { get; set; }
	
	[NotMapped]
	public decimal FinalPrice
	{
		get
		{
			if (DiscountPercentage == null || DiscountPercentage <= 0)
				return Price;

			return Price - (Price * DiscountPercentage.Value / 100m);
		}
	}

	public int? DiscountPercentage { get; set; } = 0;

	public bool? IsFeatured { get; set; } = false;
	
	public List<Review>? Reviews { get; set; }
	
	[NotMapped]
	public FormFile? FormFile { get; set; }
}