using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.ReleaseNotes;

internal static class ReleaseNoteMappings
{
    public static ReleaseNoteSummaryDto ToSummaryDto(this ReleaseNote note) =>
        new(note.Id, note.Version, note.IsPreRelease, note.Title, note.IsPublished, note.PublishedAt, note.UpdatedAt);

    public static ReleaseNoteDto ToDto(this ReleaseNote note) =>
        new(
            note.Id,
            note.ApplicationId,
            note.Version,
            note.IsPreRelease,
            note.Title,
            note.ContentHtml,
            note.IsPublished,
            note.PublishedAt,
            note.CreatedAt,
            note.UpdatedAt);
}
