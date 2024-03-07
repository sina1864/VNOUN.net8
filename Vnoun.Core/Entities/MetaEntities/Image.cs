using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities.MetaEntities;

public class Image : Entity
{
    [Ignore]
    [JsonPropertyName("_id")]
    public string _id
    {
        get
        {
            return ID;
        }
    }

    [JsonPropertyName("small_image")]
    [Field("small_image")]
    public string SmallImage { get; set; }

    [JsonPropertyName("medium_image")]
    [Field("medium_image")]
    public string MediumImage { get; set; }

    [JsonPropertyName("large_image")]
    [Field("large_image")]
    public string LargeImage { get; set; }
}