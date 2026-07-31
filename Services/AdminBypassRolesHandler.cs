using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Family_and_Spa_Wellness.Services;

// Admins get full access to every role-restricted page, regardless of which
// specific role a page targets (e.g. StaffDashboard.razor's
// [Authorize(Roles = "Provider")]). Implemented as an additional handler for
// the framework's built-in RolesAuthorizationRequirement - which is what
// every [Authorize(Roles = "...")] attribute compiles down to - rather than
// listing "Admin" in each attribute individually. Authorization handlers for
// the same requirement are OR'd together, so this covers every existing and
// future Roles-restricted page automatically.
public class AdminBypassRolesHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
