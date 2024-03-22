using Microsoft.AspNetCore.Authorization;

namespace PhotoLibraryBackend.Tests;

public class FakeAuthorizationHandler : AuthorizationHandler<ShouldHaveConfirmedEmailRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldHaveConfirmedEmailRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
