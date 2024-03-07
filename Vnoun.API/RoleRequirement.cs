using Microsoft.AspNetCore.Authorization;

namespace Vnoun.API;

public class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(string role)
    {
        Role = role;
    }

    public string Role { get; }
}