using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Vnoun.Application.Responses.MetaResponses;

namespace Vnoun.Application.Responses.Event;

public class EventResponseDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Summary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime StartsIn { get; set; }

    public DateTime EndsIn { get; set; }

    public List<CoverImageResponse> CoverImage { get; set; }

    [JsonPropertyName("__v")]
    public int Version { get; set; } = 0;
}