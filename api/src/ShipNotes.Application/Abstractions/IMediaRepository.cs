using ShipNotes.Domain.Media;

namespace ShipNotes.Application.Abstractions;

public interface IMediaRepository
{
    Task<MediaData?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Media of the application with the given ids, tracked for update.</summary>
    Task<IReadOnlyList<MediaData>> ListAsync(Guid applicationId, IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

    /// <summary>Media attached to the release note, tracked for update.</summary>
    Task<IReadOnlyList<MediaData>> ListByReleaseNoteAsync(Guid releaseNoteId, CancellationToken cancellationToken);

    void Add(MediaData media);

    void RemoveRange(IEnumerable<MediaData> media);
}
