using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Vnoun.Application.Responses.MetaResponses;

namespace Vnoun.Application.Responses.Post;

public class PostResponseDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("publisher")]
    public Notifications.NotificationUserResponseDto Publisher { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("pinned")]
    public bool Pinned { get; set; }

    [JsonPropertyName("coverImage")]
    public List<PhotoResponseDto> CoverImage { get; set; }

    [JsonPropertyName("images")]
    public List<PhotoResponseDto> Images { get; set; }
}