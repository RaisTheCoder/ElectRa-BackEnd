using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Models;

[
	Index(nameof(_Name), IsUnique = true),
	Index(nameof(Slug), IsUnique = true)
]
public class Category
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string Slug { get; set; }
	
	[DefaultValue("true")]
	public bool? Enabled { get; set; }
	public List<SubCategory>? SubCategories { get; set; }
}