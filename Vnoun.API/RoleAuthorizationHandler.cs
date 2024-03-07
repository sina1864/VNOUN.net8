using Microsoft.AspNetCore.Authorization;
using Vnoun.Core.Repositories;

namespace Vnoun.API;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IUserRepository _userRepository;
    public RoleAuthorizationHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var userRole = "admin";

        if (userRole == requirement.Role)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}