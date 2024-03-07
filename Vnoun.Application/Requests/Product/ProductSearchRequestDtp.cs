using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Product;

public class ProductSearchRequestDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}