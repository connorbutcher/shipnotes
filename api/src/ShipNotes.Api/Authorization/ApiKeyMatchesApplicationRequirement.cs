using Microsoft.AspNetCore.Authorization;

namespace ShipNotes.Api.Authorization;

/// <summary>The API key must have been issued to the application in the request's route.</summary>
internal sealed class ApiKeyMatchesApplicationRequirement : IAuthorizationRequirement
{
    public const string RouteValueName = "applicationId";
}
