using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Post;

public class PostUpdateRequestDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("publisherId")]
    public string? PublisherId { get; set; } = "";

    [JsonPropertyName("pinned")]
    public bool? Pinned { get; set; } = false;

    [JsonPropertyName("coverImage")]
    public IFormFileCollection? CoverImage { get; set; }

    [JsonPropertyName("images")]
    public IFormFileCollection? Images { get; set; }
}