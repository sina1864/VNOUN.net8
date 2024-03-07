using MongoDB.Bson.Serialization.Attributes;

namespace Vnoun.Core.Entities.MetaEntities;

public class Links
{
    [BsonElement("facebook")]
    public string Facebook { get; set; }

    [BsonElement("gmail")]
    public string Gmail { get; set; }

    [BsonElement("twitter")]
    public string Twitter { get; set; }

    [BsonElement("github")]
    public string Github { get; set; }

    [BsonElement("text")]
    public string Text { get; set; }
}