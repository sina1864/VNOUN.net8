using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Application.Responses.Product;

public class ProductResponseDto
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

    public string Name { get; set; }

    public string Gender { get; set; }

    public string Description { get; set; }

    public string Type { get; set; }

    public decimal RatingsAverage { get; set; } = 4.5M;

    public int RatingsQuantity { get; set; }

    public string Category { get; set; }

    [JsonPropertyName("collection_season")]
    public ICollection<string> CollectionSeason { get; set; } = new List<string>();

    public ICollection<string> Tags { get; set; } = new List<string>();

    [JsonPropertyName("global_discount")]
    public decimal GlobalDiscount { get; set; }

    public List<Color> Colors { get; set; } = new();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public int NumberOfViewers { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("viewers")]
    public List<string> ViewerIds { get; set; } = new List<string>();

    [JsonPropertyName("buyers")]
    public List<string> BuyersId { get; set; } = new List<string>();

    [JsonPropertyName("__v")]
    public string Version { get; set; }
}