using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities.MetaEntities;

public class Photo : Entity
{
    [Field("small_image")]
    [JsonPropertyName("small_image")]
    public string SmallImage { get; set; }

    [Field("medium_image")]
    [JsonPropertyName("medium_image")]
    public string MediumImage { get; set; }

    [Field("large_image")]
    [JsonPropertyName("large_image")]
    public string LargeImage { get; set; }
}