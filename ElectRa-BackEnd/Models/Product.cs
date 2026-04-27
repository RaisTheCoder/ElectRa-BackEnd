using System.ComponentModel.DataAnnotations.Schema;

namespace ElectRa_BackEnd.Models;

public class Product
{
	public long Id { get; set; }
	public string Title { get; set; }
	public string Thumbnail { get; set; }
	
	[ForeignKey("Categories")]
	public long CategoryId { get; set; }

	public Category Category { get; set; }
	
	public long BrandId { get; set; }
	public Brand Brand { get; set; }

	public DateTime Date { get; set; }

	public decimal Rating
	{
		get;
		set
		{
			if (value > 5)
			{
				Rating = 5;
				return;
			}

			Rating = value;
		}
	}

	public decimal Price { get; set; } 
	public List<Review> Reviews { get; set; }
	
	[NotMapped]
	public FormFile FormFile { get; set; }
}