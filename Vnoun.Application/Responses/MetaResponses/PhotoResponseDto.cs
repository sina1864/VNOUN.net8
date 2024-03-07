using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.MetaResponses;

public class PhotoResponseDto
{
    public string _id { get; set; }

    [JsonPropertyName("small_image")]
    public string SmallImage { get; set; }

    [JsonPropertyName("medium_image")]
    public string MediumImage { get; set; }

    [JsonPropertyName("large_image")]
    public string LargeImage { get; set; }
}