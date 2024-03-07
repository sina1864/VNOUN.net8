using Newtonsoft.Json;
using Vnoun.Application.Responses.MetaResponses;

namespace Vnoun.Application.Responses.User;

public class DetailedUserResponseDto
{
    public string _id { get; set; }
    public string id
    {
        get
        {
            return _id;
        }
    }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("photos")]
    public IEnumerable<PhotoResponseDto> Photos { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; } = "User";

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; } = true;

    [JsonProperty("passwordResetToken")]
    public string? PasswordResetToken { get; set; }

    [JsonProperty("passwordResetExpires")]
    public DateTime? PasswordResetExpires { get; set; }

    [JsonProperty("__v")]
    public int Version { get; set; }
}