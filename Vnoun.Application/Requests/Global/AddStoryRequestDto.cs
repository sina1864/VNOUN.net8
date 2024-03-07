using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Global;

public class AddStoryRequestDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("photo")]
    public IFormFileCollection Photo { get; set; }
}