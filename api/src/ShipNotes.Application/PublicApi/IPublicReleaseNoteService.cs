namespace ShipNotes.Application.PublicApi;

/// <summary>Read-only access to published release notes for the calling application.</summary>
public interface IPublicReleaseNoteService
{
    Task<PublicApplicationDto> GetApplicationAsync(Guid applicationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PublicReleaseNoteDto>> ListAsync(Guid applicationId, PublicReleaseNotesQuery query, CancellationToken cancellationToken);

    Task<IReadOnlyList<PublicReleaseNoteDto>> ListBetweenAsync(Guid applicationId, PublicReleaseNotesRangeQuery query, CancellationToken cancellationToken);

    /// <summary>The newest published note, or the newest with any of <paramref name="category"/> when given.</summary>
    Task<PublicReleaseNoteDto> GetLatestAsync(
        Guid applicationId,
        bool includePreReleases,
        IReadOnlyList<string>? category,
        CancellationToken cancellationToken);

    /// <summary>The note for one version; <paramref name="category"/> narrows which of its sections are returned.</summary>
    Task<PublicReleaseNoteDto> GetByVersionAsync(
        Guid applicationId,
        string version,
        IReadOnlyList<string>? category,
        CancellationToken cancellationToken);
}
