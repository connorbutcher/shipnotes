using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.Abstractions;

public interface IReleaseNoteRepository
{
    Task<IReadOnlyList<ReleaseNote>> ListAsync(ReleaseNoteQuery query, CancellationToken cancellationToken);

    Task<ReleaseNote?> FindAsync(Guid applicationId, Guid id, CancellationToken cancellationToken);

    Task<ReleaseNote?> FindByVersionAsync(Guid applicationId, string versionSortKey, bool publishedOnly, CancellationToken cancellationToken);

    Task<bool> VersionExistsAsync(Guid applicationId, string versionSortKey, Guid? excludingId, CancellationToken cancellationToken);

    void Add(ReleaseNote note);

    void Remove(ReleaseNote note);
}
