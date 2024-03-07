using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class SignupRequestDto
{
    [Required(ErrorMessage = "A user must have a name")]
    [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "A user must have an email")]
    [EmailAddress(ErrorMessage = "Please provide a valid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password")]
    [MinLength(8, ErrorMessage = "The password length should be at least 8 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "The passwords do not match")]
    public string PasswordConfirm { get; set; }
}