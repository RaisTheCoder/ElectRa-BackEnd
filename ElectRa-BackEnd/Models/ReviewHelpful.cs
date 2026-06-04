using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ElectRa_BackEnd.Models;

public class ReviewHelpful
{
	public long UserId { get; set; }
	public long ReviewId { get; set; }
	public Review Review { get; set; }
}