namespace ShipNotes.Application.PublicApi;

public sealed record PublicApplicationDto(string Name, string? IconUrl);

public sealed record PublicReleaseNoteDto(
    string Version,
    bool IsPreRelease,
    string Title,
    string ContentHtml,
    DateTimeOffset PublishedAt);

/// <summary>Query options for consuming applications.</summary>
/// <param name="After">Only versions newer than this one, e.g. the version the user last saw.</param>
/// <param name="UpTo">Only versions up to and including this one, e.g. the version currently installed.</param>
public sealed record PublicReleaseNotesQuery(
    string? After = null,
    string? UpTo = null,
    bool IncludePreReleases = false,
    int Limit = PublicReleaseNotesQuery.DefaultLimit)
{
    public const int DefaultLimit = 20;
    public const int MaxLimit = 100;
}

/// <summary>
/// A version range for "what changed between the version I had and the version I have now".
/// <paramref name="From"/> is exclusive unless <paramref name="IncludeFrom"/> is set; <paramref name="To"/> is inclusive.
/// Versions are compared by semver precedence (1.10.0 is after 1.9.0; 2.0.0-beta.1 is before 2.0.0).
/// </summary>
public sealed record PublicReleaseNotesRangeQuery(
    string? From = null,
    string? To = null,
    bool IncludeFrom = false,
    bool IncludePreReleases = false);
