using Microsoft.AspNetCore.Http;

namespace Vnoun.Application.Requests.Event;

public class EventUpdateRequestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public DateTime? StartsIn { get; set; }
    public DateTime? EndsIn { get; set; }
    public IFormFileCollection? CoverImage { get; set; }
}