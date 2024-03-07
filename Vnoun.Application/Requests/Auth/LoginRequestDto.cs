using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "A user must have an email")]
    [EmailAddress(ErrorMessage = "Please provide a valid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password")]
    [MinLength(8, ErrorMessage = "The password length should be at least 8 characters")]
    public string Password { get; set; }
}