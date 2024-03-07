using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Product;

public class RemoveDiscountsRequestDto
{
    [JsonPropertyName("colorIds")]
    public List<string> ColorIds { get; set; }
}