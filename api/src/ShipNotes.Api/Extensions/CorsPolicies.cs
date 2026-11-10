namespace ShipNotes.Api.Extensions;

internal static class CorsPolicies
{
    /// <summary>The ShipNotes admin UI only.</summary>
    public const string AdminUi = "AdminUi";

    /// <summary>Any origin, so consuming apps can call the public API from the browser.</summary>
    public const string PublicApi = "PublicApi";
}
