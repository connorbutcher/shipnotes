using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.ReleaseNotes;

internal static class ReleaseNoteMappings
{
    public static ReleaseNoteSummaryDto ToSummaryDto(this ReleaseNote note) =>
        new(
            note.Id,
            note.Version,
            note.IsPreRelease,
            note.Title,
            [.. note.Categories],
            note.IsPublished,
            note.PublishedAt,
            note.UpdatedAt);

    public static ReleaseNoteDto ToDto(this ReleaseNote note) =>
        new(
            note.Id,
            note.ApplicationId,
            note.Version,
            note.IsPreRelease,
            note.Title,
            note.SummaryHtml,
            [.. note.Sections.Select(s => new ReleaseNoteSectionDto(s.Category, s.ContentHtml))],
            note.IsPublished,
            note.PublishedAt,
            note.CreatedAt,
            note.UpdatedAt);

    /// <summary>The request's content as written, before sanitising.</summary>
    public static ReleaseNoteContent ToContent(this SaveReleaseNoteRequest request) =>
        new(
            request.SummaryHtml ?? string.Empty,
            [.. (request.Sections ?? []).Select(s => new ReleaseNoteSectionContent(s.Category, s.ContentHtml))]);
}
