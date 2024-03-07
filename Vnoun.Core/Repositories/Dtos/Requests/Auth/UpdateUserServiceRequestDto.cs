namespace Vnoun.Core.Repositories.Dtos.Requests.Auth;

public class UpdateUserServiceRequestDto
{
    public string? Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public List<string>? Image { get; set; }
}
