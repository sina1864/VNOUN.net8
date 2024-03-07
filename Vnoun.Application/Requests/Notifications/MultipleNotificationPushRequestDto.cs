using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Notifications;

public class MultipleNotificationPushRequestDto
{
    [JsonPropertyName("users")]
    public List<string> UserIds { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}