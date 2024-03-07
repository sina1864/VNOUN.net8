using System.Text.Json.Serialization;

namespace Vnoun.Application.Requests.Wishlist;

public class DeleteWishlistRequestDto
{
    [JsonPropertyName("ids")]
    public List<string> ids { get; set; }
}