using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.MetaResponses;

public class CoverImageResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }

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