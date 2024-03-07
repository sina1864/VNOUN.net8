using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("categories")]
public class Category : Entity
{
    [Field("title")]
    public string? Title { get; set; }

    [Field("order")]
    public int? Order { get; set; }

    [Field("heading")]
    public string? Heading { get; set; }

    [Field("subHeading")]
    public string? SubHeading { get; set; }

    [Field("filterData")]
    public List<FilterData>? FilterData { get; set; }

    [Field("photo")]
    public List<Photo>? Photo { get; set; }

    [Field("active")]
    public bool? Active { get; set; } = false;

    [Field("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [Field("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Field("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}