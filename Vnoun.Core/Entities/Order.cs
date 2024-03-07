using MongoDB.Bson;
using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities;

public class Order : Entity
{
    [Field("name")]
    public string Name { get; set; }

    [Field("image")]
    public string Image { get; set; }

    [Field("color")]
    public string Color { get; set; }

    [Field("size")]
    public string Size { get; set; }

    [Field("numberOfOrders")]
    public int NumberOfOrders { get; set; }

    [Field("product")]
    [JsonIgnore]
    public ObjectId ProductId { get; set; }

    [Ignore]
    [JsonPropertyName("product")]
    public string pId
    {
        get
        {
            return ProductId.ToString();
        }
    }

    [Ignore]
    public string _id
    {
        get
        {
            return ID.ToString();
        }
    }
}