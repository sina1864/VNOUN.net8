using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.MetaResponses;

public class CoverImageCreateResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [JsonPropertyName("small_image")]
    [BsonElement("small_image")]
    public string SmallImage { get; set; }

    [JsonPropertyName("medium_image")]
    [BsonElement("medium_image")]
    public string MediumImage { get; set; }

    [JsonPropertyName("large_image")]
    [BsonElement("large_image")]
    public string LargeImage { get; set; }
}