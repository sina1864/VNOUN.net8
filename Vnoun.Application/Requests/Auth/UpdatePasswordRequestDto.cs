using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Auth;

public class UpdatePasswordRequestDto
{
    [Required(ErrorMessage = "Please enter your password")]
    [MinLength(8, ErrorMessage = "The password length should be at least 8 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "The passwords do not match")]
    public string PasswordConfirm { get; set; }

    [Required(ErrorMessage = "Please enter your current password")]
    public string PasswordCurrent { get; set; }
}