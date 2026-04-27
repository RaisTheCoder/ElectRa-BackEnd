using System.Text.Json.Serialization;

namespace ElectRa_BackEnd.Models;

public class Category
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string Slug { get; set; }
	
	[JsonIgnore]
	public List<Product> Products { get; set; }
	
	[JsonIgnore]
	public List<SubCategory> SubCategories { get; set; }
}