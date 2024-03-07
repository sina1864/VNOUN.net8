using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class UpdateUserByAdminRequest
{
    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("phone")]
    [RegularExpression("^\\(\\d{3}\\)\\s\\d{3}-\\d{4}$", ErrorMessage = "Please provide a valid phone number")]
    public string? Phone { get; set; }

    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("active")]
    public bool? Active { get; set; }
}