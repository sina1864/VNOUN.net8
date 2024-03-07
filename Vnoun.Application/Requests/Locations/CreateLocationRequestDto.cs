using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Locations;

public class CreateLocationRequestDto
{
    [JsonPropertyName("information")]
    public LocationInfoRequestDto Information { get; set; }

    [RegularExpression("\\d{3}-\\d{3}-\\d{4}")]
    public string Phone { get; set; }
}

public class LocationInfoRequestDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; } = "Point";

    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}