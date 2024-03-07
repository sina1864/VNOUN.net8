using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string Email { get; set; }
}