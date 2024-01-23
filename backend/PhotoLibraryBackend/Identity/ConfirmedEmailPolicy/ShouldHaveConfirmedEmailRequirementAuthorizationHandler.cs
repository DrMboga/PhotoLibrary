using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PhotoLibraryBackend;

/// <summary>
/// This policy reads the current user properties from Identity database.
/// And checks if user has "EmailConfirmed" flag
/// Email sender should be implemented, but for simplicity, we can hack this like this: 
/// UPDATE public."AspNetUsers"	SET "EmailConfirmed"=true WHERE "Email"='mike@fake.com';
/// </summary>
public class ShouldHaveConfirmedEmailRequirementAuthorizationHandler :
    AuthorizationHandler<ShouldHaveConfirmedEmailRequirement>
{
    private readonly UserManager<IdentityUser> _userManager;

    public ShouldHaveConfirmedEmailRequirementAuthorizationHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldHaveConfirmedEmailRequirement requirement)
    {
        if (context?.User?.Identity?.Name == null)
        {
            return;
        }
        var currentUser = await _userManager.FindByEmailAsync(context.User.Identity.Name);
        if (currentUser == null)
        {
            return;
        }
        if (context.User.Identity.IsAuthenticated && currentUser.EmailConfirmed)
        {
            context.Succeed(requirement);
        }
    }
}
