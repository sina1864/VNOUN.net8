using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Vnoun.Application.Requests.Category;

public class CategoryUpdateRequestDto
{
    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("order")]
    public int? Order { get; set; }

    [JsonProperty("heading")]
    public string? Heading { get; set; }

    [JsonProperty("subHeading")]
    public string? SubHeading { get; set; }

    [JsonProperty("photo")]
    public IFormFileCollection? Photo { get; set; }
}