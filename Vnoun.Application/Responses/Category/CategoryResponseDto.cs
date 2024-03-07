using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Application.Responses.Category;

public class CategoryResponseDto
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

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("heading")]
    public string Heading { get; set; }

    [JsonPropertyName("subHeading")]
    public string SubHeading { get; set; }

    [JsonPropertyName("filterData")]
    public List<FilterData>? FilterData { get; set; } = new List<FilterData>();

    [JsonPropertyName("photo")]
    public List<Photo> Photo { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("__v")]
    [BsonIgnoreIfNull]
    public int Version { get; set; }
}