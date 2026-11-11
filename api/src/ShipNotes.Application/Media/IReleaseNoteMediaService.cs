using ShipNotes.Domain.Media;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.Media;

/// <summary>Keeps release note HTML limited to uploaded media and the MediaData records in step with it.</summary>
internal interface IReleaseNoteMediaService
{
    /// <summary>
    /// Sanitises note HTML, keeping only images that are media uploaded to this application
    /// (and not already used by another note; pass null for a new note), with their links rewritten to the canonical URL.
    /// </summary>
    Task<NoteContent> PrepareContentAsync(Guid applicationId, Guid? releaseNoteId, string html, CancellationToken cancellationToken);

    /// <summary>
    /// Attaches the media the note now uses and removes the records of media it no longer uses.
    /// Returns the removed media so their files can be deleted once the change is saved.
    /// </summary>
    Task<IReadOnlyList<MediaData>> SyncAsync(ReleaseNote note, NoteContent content, CancellationToken cancellationToken);

    /// <summary>Removes the records of all media attached to the note and returns them.</summary>
    Task<IReadOnlyList<MediaData>> DetachAllAsync(Guid releaseNoteId, CancellationToken cancellationToken);

    void DeleteFiles(IEnumerable<MediaData> media);
}

internal sealed record NoteContent(string Html, IReadOnlyList<MediaData> Media);
