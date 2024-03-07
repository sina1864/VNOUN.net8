using MongoDB.Bson.Serialization.Attributes;

namespace Vnoun.Core.Entities.MetaEntities;

public class Store
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("logo")]
    public List<Image> Logo { get; set; }
}