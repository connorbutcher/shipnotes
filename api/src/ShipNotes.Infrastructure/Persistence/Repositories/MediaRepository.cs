using Microsoft.EntityFrameworkCore;
using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.Media;

namespace ShipNotes.Infrastructure.Persistence.Repositories;

internal sealed class MediaRepository(ShipNotesDbContext db) : IMediaRepository
{
    public Task<MediaData?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.Media.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<MediaData>> ListAsync(
        Guid applicationId,
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken) =>
        await db.Media.Where(m => m.ApplicationId == applicationId && ids.Contains(m.Id)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MediaData>> ListByReleaseNoteAsync(Guid releaseNoteId, CancellationToken cancellationToken) =>
        await db.Media.Where(m => m.ReleaseNoteId == releaseNoteId).ToListAsync(cancellationToken);

    public void Add(MediaData media) => db.Media.Add(media);

    public void RemoveRange(IEnumerable<MediaData> media) => db.Media.RemoveRange(media);
}
