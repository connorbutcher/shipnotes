using Microsoft.AspNetCore.Authorization;
using ShipNotes.Api.Authentication;

namespace ShipNotes.Api.Authorization;

/// <summary>
/// Succeeds only when the authenticated key's application id equals the <c>{applicationId}</c>
/// route value, so a key for one application can never read another application's notes.
/// </summary>
internal sealed class ApiKeyMatchesApplicationHandler(IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<ApiKeyMatchesApplicationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApiKeyMatchesApplicationRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext ?? httpContextAccessor.HttpContext;
        var routeValue = httpContext?.GetRouteValue(ApiKeyMatchesApplicationRequirement.RouteValueName)?.ToString();

        if (Guid.TryParse(routeValue, out var requestedId)
            && context.User.TryGetApplicationId(out var keyApplicationId)
            && keyApplicationId == requestedId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
