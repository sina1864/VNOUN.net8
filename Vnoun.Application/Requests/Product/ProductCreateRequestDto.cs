using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Product;

public class ProductCreateRequestDto
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string? Gender { get; set; }

    public string? AgeGroup { get; set; }

    public string? Category { get; set; }

    [JsonPropertyName("collection_season")]
    public List<string>? CollectionSeason { get; set; }

    public string[]? Tags { get; set; }

    [JsonPropertyName("global_discount")]
    public double? GlobalDiscount { get; set; }

    [JsonPropertyName("__v")]
    public int Version { get; set; }
}