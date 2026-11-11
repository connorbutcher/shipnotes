using Microsoft.EntityFrameworkCore;
using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Infrastructure.Persistence.Repositories;

internal sealed class ReleaseNoteRepository(ShipNotesDbContext db) : IReleaseNoteRepository
{
    public async Task<IReadOnlyList<ReleaseNote>> ListAsync(ReleaseNoteQuery query, CancellationToken cancellationToken)
    {
        var notes = db.ReleaseNotes.AsNoTracking().Where(n => n.ApplicationId == query.ApplicationId);

        if (query.PublishedOnly)
        {
            notes = notes.Where(n => n.PublishedAt != null);
        }

        if (!query.IncludePreReleases)
        {
            notes = notes.Where(n => !n.IsPreRelease);
        }

        if (query.AfterSortKey is { } after)
        {
            notes = notes.Where(n => n.VersionSortKey.CompareTo(after) > 0);
        }

        if (query.FromSortKey is { } from)
        {
            notes = notes.Where(n => n.VersionSortKey.CompareTo(from) >= 0);
        }

        if (query.UpToSortKey is { } upTo)
        {
            notes = notes.Where(n => n.VersionSortKey.CompareTo(upTo) <= 0);
        }

        notes = notes.OrderByDescending(n => n.VersionSortKey);

        if (query.Limit is { } limit)
        {
            notes = notes.Take(limit);
        }

        return await notes.ToListAsync(cancellationToken);
    }

    public Task<ReleaseNote?> FindAsync(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        db.ReleaseNotes.FirstOrDefaultAsync(n => n.ApplicationId == applicationId && n.Id == id, cancellationToken);

    public Task<ReleaseNote?> FindByVersionAsync(
        Guid applicationId,
        string versionSortKey,
        bool publishedOnly,
        CancellationToken cancellationToken) =>
        db.ReleaseNotes.AsNoTracking().FirstOrDefaultAsync(
            n => n.ApplicationId == applicationId
                && n.VersionSortKey == versionSortKey
                && (!publishedOnly || n.PublishedAt != null),
            cancellationToken);

    public Task<bool> VersionExistsAsync(
        Guid applicationId,
        string versionSortKey,
        Guid? excludingId,
        CancellationToken cancellationToken) =>
        db.ReleaseNotes.AnyAsync(
            n => n.ApplicationId == applicationId && n.VersionSortKey == versionSortKey && n.Id != excludingId,
            cancellationToken);

    public void Add(ReleaseNote note) => db.ReleaseNotes.Add(note);

    public void Remove(ReleaseNote note) => db.ReleaseNotes.Remove(note);
}
