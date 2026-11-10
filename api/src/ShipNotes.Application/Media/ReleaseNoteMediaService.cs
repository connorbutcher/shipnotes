using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.Media;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.Media;

internal sealed class ReleaseNoteMediaService(
    IMediaRepository mediaRepository,
    IMediaStorage storage,
    IMediaUrlProvider urls,
    IHtmlContentSanitizer sanitizer,
    TimeProvider clock) : IReleaseNoteMediaService
{
    public async Task<NoteContent> PrepareContentAsync(
        Guid applicationId,
        Guid? releaseNoteId,
        string html,
        CancellationToken cancellationToken)
    {
        var candidateIds = MediaRoutes.FindMediaIds(html);
        var available = candidateIds.Count == 0
            ? []
            : (await mediaRepository.ListAsync(applicationId, [.. candidateIds], cancellationToken))
                .Where(m => m.ReleaseNoteId is null || m.ReleaseNoteId == releaseNoteId)
                .ToDictionary(m => m.Id);

        var used = new Dictionary<Guid, MediaData>();
        var sanitized = sanitizer.Sanitize(html, src =>
        {
            if (MediaRoutes.TryGetMediaId(src, out var id) && available.TryGetValue(id.Value, out var media))
            {
                used[media.Id] = media;
                return urls.GetUrl(media);
            }

            return null;
        });

        return new NoteContent(sanitized, [.. used.Values]);
    }

    public async Task<IReadOnlyList<MediaData>> SyncAsync(ReleaseNote note, NoteContent content, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        foreach (var media in content.Media)
        {
            media.AttachTo(note.Id, now);
        }

        var usedIds = content.Media.Select(m => m.Id).ToHashSet();
        var unused = (await mediaRepository.ListByReleaseNoteAsync(note.Id, cancellationToken))
            .Where(m => !usedIds.Contains(m.Id))
            .ToList();

        mediaRepository.RemoveRange(unused);
        return unused;
    }

    public async Task<IReadOnlyList<MediaData>> DetachAllAsync(Guid releaseNoteId, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.ListByReleaseNoteAsync(releaseNoteId, cancellationToken);
        mediaRepository.RemoveRange(media);
        return media;
    }

    public void DeleteFiles(IEnumerable<MediaData> media)
    {
        foreach (var item in media)
        {
            storage.Delete(item.StoragePath);
        }
    }
}
