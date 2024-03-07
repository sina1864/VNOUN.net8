using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.Cards;
public class CardsResponseDto
{
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    public string User { get; set; }

    public Product.ProductResponseDto Product { get; set; }

    public string Color { get; set; }

    public string Size { get; set; }

    public int NumberOfOrders { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("__v")]
    public int? Version { get; set; } = 0;
}