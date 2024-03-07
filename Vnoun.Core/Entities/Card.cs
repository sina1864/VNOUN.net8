using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities;

[Collection("cards")]
public class Card : Entity
{
    [Field("user")]
    [JsonPropertyName("user")]
    public string UserId { get; set; }

    [Field("product")]
    [JsonPropertyName("product")]
    public string ProductId { get; set; }

    [Field("color")]
    public string Color { get; set; }

    [Field("size")]
    public string? Size { get; set; }

    [Field("numberOfOrders")]
    public int NumberOfOrders { get; set; }

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Ignore]
    [JsonIgnore]
    public User User { get; set; }

    [Ignore]
    [JsonIgnore]
    public Product Product { get; set; }

    [Field("__v")]
    [Ignore]
    [JsonPropertyName("__v")]
    public int? Version { get; set; }
}