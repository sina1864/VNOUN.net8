using MongoDB.Entities;

namespace Vnoun.Core.Entities;

[Collection("reviews")]
public class Review : Entity
{
    [Field("description")]
    public string Description { get; set; }

    [Field("rating")]
    public decimal Rating { get; set; }

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Field("user")]
    public string UserId { get; set; }

    [Field("product")]
    public string ProductId { get; set; }

    [Ignore]
    public User User { get; set; }

    [Ignore]
    public Product Product { get; set; }

    [Field("__v")]
    public int Version { get; set; } = 0;
}