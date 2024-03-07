using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Product;

public class AddGlobalDiscountRequestDto
{
    [JsonPropertyName("ids")]
    public List<string> Ids { get; set; }

    [JsonPropertyName("discount")]
    public double Discount { get; set; }
}