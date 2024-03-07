using MongoDB.Entities;

namespace Vnoun.Core.Entities.MetaEntities;

public class Shipping : Entity
{
    [Field("address")]
    public Address Address { get; set; }

    [Field("phone")]
    public string Phone { get; set; }

    [Field("name")]
    public string Name { get; set; }
}