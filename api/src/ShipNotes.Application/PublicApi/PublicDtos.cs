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
