using ShipNotes.Domain.Common;

namespace ShipNotes.Domain.Media;

/// <summary>
/// An uploaded media file (stored outside the database) and the release note that uses it.
/// Media starts unattached when uploaded from the editor and is attached when a note that references it is saved.
/// </summary>
public sealed class MediaData : Entity
{
    private MediaData()
    {
    }

    public Guid ApplicationId { get; private set; }

    public Guid? ReleaseNoteId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeBytes { get; private set; }

    /// <summary>Location of the file relative to the media storage root.</summary>
    public string StoragePath { get; private set; } = string.Empty;

    public static MediaData Create(
        Guid id,
        Guid applicationId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset now)
    {
        var media = new MediaData
        {
            Id = id,
            ApplicationId = applicationId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StoragePath = storagePath,
        };
        media.Touch(now);
        return media;
    }

    public void AttachTo(Guid releaseNoteId, DateTimeOffset now)
    {
        if (ReleaseNoteId == releaseNoteId)
        {
            return;
        }

        ReleaseNoteId = releaseNoteId;
        Touch(now);
    }
}
