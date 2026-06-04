using System.Text.Json.Serialization;
using ElectRa_BackEnd.Helpers;

namespace ElectRa_BackEnd.DataTransferObjects;

public class UpdateOrderAdminDTO
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus? Status { get; set; }
    public string? TrackingNumber { get; set; }
    public string? AdminNote { get; set; }
}