namespace ShipNotes.Application.Abstractions;

/// <summary>Filters for listing release notes, newest version first.</summary>
/// <param name="AfterSortKey">Only versions strictly newer than this sort key.</param>
/// <param name="UpToSortKey">Only versions up to and including this sort key.</param>
public sealed record ReleaseNoteQuery(
    Guid ApplicationId,
    bool PublishedOnly,
    bool IncludePreReleases,
    string? AfterSortKey = null,
    string? UpToSortKey = null,
    int? Limit = null);
