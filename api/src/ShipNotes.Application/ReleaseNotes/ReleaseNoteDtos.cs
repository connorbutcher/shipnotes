using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.ReleaseNotes;

public sealed record ReleaseNoteSummaryDto(
    Guid Id,
    string Version,
    bool IsPreRelease,
    string Title,
    IReadOnlyList<ReleaseNoteCategory> Categories,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    DateTimeOffset UpdatedAt);

public sealed record ReleaseNoteDto(
    Guid Id,
    Guid ApplicationId,
    string Version,
    bool IsPreRelease,
    string Title,
    string SummaryHtml,
    IReadOnlyList<ReleaseNoteSectionDto> Sections,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>The rich text for one category of change, e.g. everything that was fixed.</summary>
public sealed record ReleaseNoteSectionDto(ReleaseNoteCategory Category, string ContentHtml);

/// <summary>
/// Creates or replaces a release note. <paramref name="SummaryHtml"/> is optional introductory text;
/// <paramref name="Sections"/> holds at most one entry per category, and sections with no content are dropped.
/// </summary>
public sealed record SaveReleaseNoteRequest(
    string Version,
    string Title,
    string? SummaryHtml,
    IReadOnlyList<ReleaseNoteSectionDto>? Sections,
    bool IsPublished);
