using Microsoft.AspNetCore.Authorization;

namespace PhotoLibraryBackend.Tests;

public class FakeAuthorizationHandler : AuthorizationHandler<ShouldHaveConfirmedEmailRequirement>
{

    public int HandlerCalledTimes { get; private set; } = 0;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldHaveConfirmedEmailRequirement requirement)
    {
        context.Succeed(requirement);
        HandlerCalledTimes++;
        return Task.CompletedTask;
    }
}
