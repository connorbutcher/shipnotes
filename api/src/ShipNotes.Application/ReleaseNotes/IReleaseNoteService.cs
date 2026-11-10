namespace ShipNotes.Application.ReleaseNotes;

public interface IReleaseNoteService
{
    Task<IReadOnlyList<ReleaseNoteSummaryDto>> ListAsync(Guid applicationId, CancellationToken cancellationToken);

    Task<ReleaseNoteDto> GetAsync(Guid applicationId, Guid id, CancellationToken cancellationToken);

    Task<ReleaseNoteDto> CreateAsync(Guid applicationId, SaveReleaseNoteRequest request, CancellationToken cancellationToken);

    Task<ReleaseNoteDto> UpdateAsync(Guid applicationId, Guid id, SaveReleaseNoteRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid applicationId, Guid id, CancellationToken cancellationToken);
}
