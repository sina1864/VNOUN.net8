using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("events")]
public class Event : Entity
{
    [Field("title")]
    public string? Title { get; set; }

    [Field("description")]
    public string? Description { get; set; }

    [Field("summary")]
    public string? Summary { get; set; }

    [Field("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [Field("startsIn")]
    public DateTime? StartsIn { get; set; }

    [Field("endsIn")]
    public DateTime? EndsIn { get; set; }

    [Field("coverImage")]
    public List<CoverImage?> CoverImage { get; set; }

    [Field("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Field("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}