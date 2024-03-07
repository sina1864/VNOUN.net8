using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities.MetaEntities;

public class Size
{
    public string _id { get; set; } = "";
    public string id { get; set; } = "";
    public int Quantity { get; set; }

    [JsonPropertyName("size")]
    public string SizeName { get; set; }
}