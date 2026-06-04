using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Models;

[
	Index(nameof(_Name), IsUnique = true),
	Index(nameof(Slug), IsUnique = true)
]
public class SubCategory
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string Slug { get; set; }
	public string? Icon { get; set; }
	public bool Enabled { get; set; } = true;
	
	public long CategoryId { get; set; }
	
	[JsonIgnore]
	public Category? Category { get; set; }
	
	[JsonIgnore]
	public List<Product>? Products { get; set; }
	
	[NotMapped]
	public FormFile? FormFile { get; set; }
}