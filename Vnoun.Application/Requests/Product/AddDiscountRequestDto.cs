using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Product;

public class AddDiscountRequestDto
{
    [JsonPropertyName("discounts")]
    public Dictionary<string, double> Discounts { get; set; }
}