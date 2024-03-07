using System.Text.Json.Serialization;
using Vnoun.Application.Responses.Notifications;

namespace Vnoun.Application.Responses.Reviews;

public class ReviewsResponseDto
{
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    public string Description { get; set; }

    public decimal Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public NotificationUserResponseDto User { get; set; }

    [JsonPropertyName("product")]
    public string ProductId { get; set; }

    [JsonPropertyName("__v")]
    public int Version { get; set; } = 0;
}