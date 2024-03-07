using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities.MetaEntities;

public class FilterData : Entity
{
    [Ignore]
    public string _id
    {
        get
        {
            return ID.ToString();
        }
    }

    [JsonPropertyName("properyName")]
    [Field("properyName")]
    public string? PropertyName { get; set; }

    [Field("values")]
    public List<string>? Values { get; set; }

    [Field("selectionStyle")]
    public string? SelectionStyle { get; set; }

    [Field("defaultValue")]
    public string? DefaultValue { get; set; }

    [Field("order")]
    public int? Order { get; set; }
}