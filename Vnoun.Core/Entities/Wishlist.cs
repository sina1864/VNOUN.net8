using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities;

public class Wishlist : Entity
{
    [ForeignKey("User")]
    [JsonPropertyName("user")]
    public string UserId { get; set; }

    [JsonIgnore]
    public User User { get; set; }

    [ForeignKey("Product")]
    [JsonPropertyName("product")]
    public string ProductId { get; set; }

    [JsonIgnore]
    public Product Product { get; set; }

    [BsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Field("__v")]
    [JsonPropertyName("__v")]
    public string Version { get; set; }

    [Ignore]
    [JsonPropertyName("_id")]
    public string _id
    {
        get
        {
            return ID.ToString();
        }
    }
}