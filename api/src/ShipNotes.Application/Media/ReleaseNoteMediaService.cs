using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common;
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
        ReleaseNoteContent content,
        CancellationToken cancellationToken)
    {
        var candidateIds = AllHtml(content).SelectMany(MediaRoutes.FindMediaIds).ToHashSet();
        var available = candidateIds.Count == 0
            ? []
            : (await mediaRepository.ListAsync(applicationId, [.. candidateIds], cancellationToken))
                .Where(m => m.ReleaseNoteId is null || m.ReleaseNoteId == releaseNoteId)
                .ToDictionary(m => m.Id);

        var used = new Dictionary<Guid, MediaData>();
        string Sanitize(string html) => sanitizer.Sanitize(html, src =>
        {
            if (MediaRoutes.TryGetMediaId(src, out var id) && available.TryGetValue(id.Value, out var media))
            {
                used[media.Id] = media;
                return urls.GetUrl(media);
            }

            return null;
        });

        var summary = Sanitize(content.SummaryHtml);
        var sections = content.Sections
            .Select(s => s with { ContentHtml = Sanitize(s.ContentHtml) })
            .Where(s => !HtmlText.IsBlank(s.ContentHtml))
            .ToList();

        return new NoteContent(new ReleaseNoteContent(HtmlText.IsBlank(summary) ? string.Empty : summary, sections), [.. used.Values]);
    }

    private static IEnumerable<string> AllHtml(ReleaseNoteContent content) =>
        [content.SummaryHtml, .. content.Sections.Select(s => s.ContentHtml)];

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
