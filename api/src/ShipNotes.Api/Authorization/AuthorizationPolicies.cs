namespace ShipNotes.Api.Authorization;

public static class AuthorizationPolicies
{
    /// <summary>
    /// Requires an <c>X-Api-Key</c> header whose key belongs to the application named by the
    /// <c>{applicationId}</c> route value.
    /// </summary>
    public const string ApplicationApiKey = "ApplicationApiKey";
}
