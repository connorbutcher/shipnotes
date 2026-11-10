using System.Security.Claims;

namespace ShipNotes.Api.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetApplicationId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(ApiKeyAuthenticationDefaults.ApplicationIdClaim), out var id)
            ? id
            : throw new InvalidOperationException("The request is not authenticated with an application API key.");
}
