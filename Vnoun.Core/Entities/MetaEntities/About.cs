using MongoDB.Bson.Serialization.Attributes;

namespace Vnoun.Core.Entities.MetaEntities;

public class About
{
    [BsonElement("heading")]
    public string Heading { get; set; }

    [BsonElement("summary")]
    public string Summary { get; set; }

    [BsonElement("coverImage")]
    public List<Image> CoverImage { get; set; }

    [BsonElement("story")]
    public List<Story> Story { get; set; }
}