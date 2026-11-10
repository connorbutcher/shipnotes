using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.PublicApi;

public sealed record PublicApplicationDto(string Name, string? IconUrl);

/// <summary>A published release note as consuming applications receive it.</summary>
/// <param name="SummaryHtml">Introductory text shown before the sections; may be empty.</param>
/// <param name="Sections">One section per category of change, in display order (breaking changes first).</param>
/// <param name="ContentHtml">
/// The summary and every section as one ready-to-render HTML fragment, each section wrapped in
/// <c>&lt;section data-category="..."&gt;</c> with an <c>&lt;h2&gt;</c> heading, for apps that don't need the structure.
/// </param>
public sealed record PublicReleaseNoteDto(
    string Version,
    bool IsPreRelease,
    string Title,
    string SummaryHtml,
    IReadOnlyList<PublicReleaseNoteSectionDto> Sections,
    string ContentHtml,
    DateTimeOffset PublishedAt);

/// <param name="Label">Display name for the category, e.g. "Breaking changes" or "Fixed".</param>
public sealed record PublicReleaseNoteSectionDto(ReleaseNoteCategory Category, string Label, string ContentHtml);

/// <summary>Query options for consuming applications.</summary>
/// <param name="After">Only versions newer than this one, e.g. the version the user last saw.</param>
/// <param name="UpTo">Only versions up to and including this one, e.g. the version currently installed.</param>
/// <param name="Category">Only notes with these categories, and only those sections of them.</param>
public sealed record PublicReleaseNotesQuery(
    string? After = null,
    string? UpTo = null,
    bool IncludePreReleases = false,
    int Limit = PublicReleaseNotesQuery.DefaultLimit,
    string[]? Category = null)
{
    public const int DefaultLimit = 20;
    public const int MaxLimit = 100;
}

/// <summary>
/// A version range for "what changed between the version I had and the version I have now".
/// <paramref name="From"/> is exclusive unless <paramref name="IncludeFrom"/> is set; <paramref name="To"/> is inclusive.
/// Versions are compared by semver precedence (1.10.0 is after 1.9.0; 2.0.0-beta.1 is before 2.0.0).
/// <paramref name="Category"/> keeps only notes with these categories, and only those sections of them.
/// </summary>
public sealed record PublicReleaseNotesRangeQuery(
    string? From = null,
    string? To = null,
    bool IncludeFrom = false,
    bool IncludePreReleases = false,
    string[]? Category = null);
