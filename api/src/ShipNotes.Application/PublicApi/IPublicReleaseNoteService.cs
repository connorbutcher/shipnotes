namespace ShipNotes.Application.PublicApi;

/// <summary>Read-only access to published release notes for the calling application.</summary>
public interface IPublicReleaseNoteService
{
    Task<PublicApplicationDto> GetApplicationAsync(Guid applicationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PublicReleaseNoteDto>> ListAsync(Guid applicationId, PublicReleaseNotesQuery query, CancellationToken cancellationToken);

    Task<PublicReleaseNoteDto> GetLatestAsync(Guid applicationId, bool includePreReleases, CancellationToken cancellationToken);

    Task<PublicReleaseNoteDto> GetByVersionAsync(Guid applicationId, string version, CancellationToken cancellationToken);
}
