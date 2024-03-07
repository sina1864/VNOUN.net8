using MongoDB.Bson.Serialization.Attributes;

namespace Vnoun.Core.Entities.MetaEntities;

public class TermsAndConditions
{
    [BsonElement("description")]
    public string Description { get; set; }
}