using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests;

public class UpdateReviewRequestDto
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("rating")]
    public int? Rating { get; set; }
}