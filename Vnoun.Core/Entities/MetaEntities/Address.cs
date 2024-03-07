using MongoDB.Entities;

namespace Vnoun.Core.Entities.MetaEntities;

public class Address : Entity
{
    [Field("country")]
    public string Country { get; set; }

    [Field("city")]
    public string City { get; set; }

    [Field("line1")]
    public string Line1 { get; set; }

    [Field("postal_code")]
    public string PostalCode { get; set; }
}