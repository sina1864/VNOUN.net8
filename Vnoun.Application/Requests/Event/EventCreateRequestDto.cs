using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Event;

public class EventCreateRequestDto
{
    [Required(ErrorMessage = "A user must have an title")]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [Required(ErrorMessage = "A user must have an description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [Required(ErrorMessage = "A user must have an summary")]
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [Required(ErrorMessage = "Event Must Has a Starting Date")]
    [JsonPropertyName("startsIn")]
    public DateTime StartsIn { get; set; }

    [Required(ErrorMessage = "Event Must Has an Ending Date")]
    [JsonPropertyName("endsIn")]
    public DateTime EndsIn { get; set; }

    [Required(ErrorMessage = "Event Must Has an Cover Image")]
    [JsonPropertyName("coverImage")]
    public IFormFileCollection CoverImage { get; set; }
}