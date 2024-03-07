using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class UpdateMeRequestDto
{
    public string? Password { get; set; }

    public string? PasswordConfirm { get; set; }

    public string? Name { get; set; }


    [EmailAddress(ErrorMessage = "Please provide a valid email")]
    public string? Email { get; set; }

    [RegularExpression("^\\(\\d{3}\\)\\s\\d{3}-\\d{4}$", ErrorMessage = "Please provide a valid phone number")]
    public string? Phone { get; set; }

    public IFormFileCollection? Photos { get; set; }

    public IFormFileCollection? Photo { get; set; }
}
