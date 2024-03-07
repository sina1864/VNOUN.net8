using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Vnoun.Application.Responses.MetaResponses;

namespace Vnoun.Application.Responses.Notifications;
public class NotificationResponse
{
    [JsonProperty("_id")]
    public string ID { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("seen")]
    public bool Seen { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("user")]
    public string User { get; set; }

    [JsonPropertyName("__v")]
    public string Version { get; set; }
}

public class NotificationResponseWithUser
{
    [JsonProperty("_id")]
    public string ID { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("seen")]
    public bool Seen { get; set; }

    [JsonProperty("link")]
    public string Link { get; set; } = "/";

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("user")]
    public NotificationUserResponseDto User { get; set; }

    [JsonPropertyName("__v")]
    public string Version { get; set; }
}

public class NotificationUserResponseDto
{
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }
    public string Name { get; set; }
    public List<PhotoResponseDto> Photo { get; set; }
}