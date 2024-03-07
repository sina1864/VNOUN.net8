using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Cards;

public class CreateCardRequestDto
{
    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }

    [JsonPropertyName("numberOfOrders")]
    public int NumberOfOrders { get; set; }
}


public class UpdateCardRequestDto
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }

    [JsonPropertyName("numberOfOrders")]
    public int? NumberOfOrders { get; set; }
}