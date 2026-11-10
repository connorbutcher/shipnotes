using System.Security.Claims;

namespace ShipNotes.Api.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static bool TryGetApplicationId(this ClaimsPrincipal user, out Guid applicationId) =>
        Guid.TryParse(user.FindFirstValue(ApiKeyAuthenticationDefaults.ApplicationIdClaim), out applicationId);
}
