using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Category;

public class FilterDataUpdateRequestDto
{
    [JsonPropertyName("properyName")]
    public string? PropertyName { get; set; }

    public List<string>? Values { get; set; }

    public string? SelectionStyle { get; set; }

    public string? DefaultValue { get; set; }

    public int? Order { get; set; }
}