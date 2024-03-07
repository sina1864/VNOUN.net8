using MongoDB.Entities;

namespace Vnoun.Core.Entities.MetaEntities;

public class Story : Entity
{
    [Field("text")]
    public string Text { get; set; }

    [Field("photo")]
    public List<Image> Photo { get; set; }
}