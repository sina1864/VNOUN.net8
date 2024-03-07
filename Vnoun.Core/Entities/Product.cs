using MongoDB.Bson;
using MongoDB.Entities;
using System.Text.Json.Serialization;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("products")]
public class Product : Entity
{
    [Field("name")]
    public string Name { get; set; }

    [Field("description")]
    public string Description { get; set; }

    [Field("type")]
    public string Type { get; set; }

    [Field("gender")]
    public string Gender { get; set; }

    [Field("age_group")]
    public string AgeGroup { get; set; }

    [Field("ratingsAverage")]
    public double RatingsAverage { get; set; } = 4.5;

    [Field("ratingsQuantity")]
    public int RatingsQuantity { get; set; }

    [Field("category")]
    public string Category { get; set; }

    [Field("collection_season")]
    public List<string> CollectionSeason { get; set; }

    [Field("tags")]
    public List<string> Tags { get; set; }

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Field("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Field("viewers")]
    public List<ObjectId> ViewersId { get; set; }

    [Field("buyers")]
    public List<ObjectId> BuyersId { get; set; }

    [Field("global_discount")]
    public double GlobalDiscount { get; set; }

    [Field("colors")]
    public List<Color> Colors { get; set; } = new();

    [Field("numberOfViewers")]
    public int NumberOfViewers { get; set; }

    [JsonIgnore]
    [Field("wishes")]
    public int wishes { get; set; } = 0;

    [JsonIgnore]
    [Field("card_adds")]
    public int card_adds { get; set; } = 0;

    [Ignore]
    public List<User> Viewers { get; set; }

    [Ignore]
    public List<string> Buyers
    {
        get
        {
            if (BuyersId == null)
            {
                return null;
            }
            return BuyersId.Select(c => c.ToString()).ToList();
        }
        set
        {
            BuyersId = value.Select(c => ObjectId.Parse(c)).ToList();
        }
    }

    [Ignore]
    public List<string> Reviews { get; set; }

    [Field("__v")]
    [Ignore]
    public int Version { get; set; }
}