using System.ComponentModel.DataAnnotations.Schema;

namespace ElectRa_BackEnd.Models;

public class Brand
{
	public long Id { get; set; }
	public string _Name { get; set; }
	public string Icon { get; set; }
	public string enabled { get; set; }
	
	[NotMapped]
	public FormFile FormFile { get; set; }
}