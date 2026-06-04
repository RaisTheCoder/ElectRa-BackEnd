using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.Models;

[Index(nameof(_Name), IsUnique = true)]
public class Brand
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string? Icon { get; set; }
	
	[DefaultValue("true")]
	public bool? Enabled { get; set; }
	
	[NotMapped]
	public FormFile? FormFile { get; set; }
}