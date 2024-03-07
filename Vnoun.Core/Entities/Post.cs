using MongoDB.Bson;
using MongoDB.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("posts")]
public class Post : Entity
{
    [Field("title")]
    public string Title { get; set; }

    [Field("description")]
    public string? Description { get; set; }

    [Field("summary")]
    public string Summary { get; set; }

    [Field("publisher")]
    [ForeignKey("users")]
    public ObjectId PublisherId { get; set; }

    [Ignore]
    public User Publisher { get; set; }

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Field("pinned")]
    [DefaultValue(false)]
    public bool Pinned { get; set; }

    [Field("coverImage")]
    public List<CoverImage> CoverImage { get; set; }

    [Field("images")]
    public List<PostImage> Images { get; set; }

    [Field("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Field("__v")]
    [Ignore]
    public int Version { get; set; }
}