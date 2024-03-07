using MongoDB.Entities;
using System.Text.Json.Serialization;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("globals")]
public class Global : Entity
{
    [Field("about")]
    public About About { get; set; }

    [Field("links")]
    public Links Links { get; set; }

    [Field("policy")]
    public Policy Policy { get; set; }

    [Field("termsAndConditions")]
    public TermsAndConditions TermsAndConditions { get; set; }

    [Field("store")]
    public Store Store { get; set; }

    [Field("__v")]
    [Ignore]
    [JsonPropertyName("__v")]
    public int Version { get; set; }
}