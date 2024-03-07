using MongoDB.Bson;
using MongoDB.Entities;

namespace Vnoun.Core.Entities;

[Collection("notifications")]
public class Notification : Entity
{
    [Field("title")]
    public string Title { get; set; }

    [Field("description")]
    public string Description { get; set; }

    [Field("seen")]
    public bool Seen { get; set; } = false;

    [Field("user")]
    public ObjectId UserId { get; set; }

    [Field("link")]
    public string Link { get; set; } = "";

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Field("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public User User { get; set; }
}