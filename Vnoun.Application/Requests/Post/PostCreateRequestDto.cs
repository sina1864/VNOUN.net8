using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Post;

public class PostCreateRequestDto
{
    [JsonPropertyName("title")]
    [Required(ErrorMessage = "Title is required")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    [JsonPropertyName("summary")]
    [Required(ErrorMessage = "Summary is required")]
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