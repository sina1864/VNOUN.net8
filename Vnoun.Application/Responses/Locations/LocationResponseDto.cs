using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.Locations;
public class LocationResponseDto
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    public LocationInfoResponseDto Information { get; set; }

    public string Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("__v")]
    public int Version { get; set; }
}

public class LocationInfoResponseDto
{
    public string Type { get; set; }
    public List<double> Coordinates { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
}