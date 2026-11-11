namespace ShipNotes.Application.ReleaseNotes;

public sealed record ReleaseNoteSummaryDto(
    Guid Id,
    string Version,
    bool IsPreRelease,
    string Title,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    DateTimeOffset UpdatedAt);

public sealed record ReleaseNoteDto(
    Guid Id,
    Guid ApplicationId,
    string Version,
    bool IsPreRelease,
    string Title,
    string ContentHtml,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record SaveReleaseNoteRequest(string Version, string Title, string ContentHtml, bool IsPublished);
