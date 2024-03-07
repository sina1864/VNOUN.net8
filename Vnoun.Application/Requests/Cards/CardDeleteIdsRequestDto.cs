using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Cards;

public class CardDeleteIdsRequestDto
{
    [JsonPropertyName("ids")]
    public List<string> CardIds { get; set; }
}