using System.ComponentModel.DataAnnotations.Schema;

namespace ElectRa_BackEnd.Models;

public class SubCategory
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string Slug { get; set; }
	
	public long CategoryId { get; set; }
	public Category Category { get; set; }
}