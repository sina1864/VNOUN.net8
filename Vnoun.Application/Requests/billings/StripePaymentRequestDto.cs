using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Billings;

public class StripePaymentRequestDto
{
    [JsonPropertyName("cardIds")]
    public List<string> CardIds { get; set; }


    [JsonPropertyName("city")]
    public string City { get; set; }


    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("line1")]
    public string Line1 { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }
}