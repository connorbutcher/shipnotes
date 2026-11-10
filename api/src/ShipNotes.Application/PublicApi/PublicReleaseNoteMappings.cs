using System.Net;
using System.Text;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.PublicApi;

internal static class PublicReleaseNoteMappings
{
    /// <summary>Maps a published note, keeping only the sections in <paramref name="categories"/> when given.</summary>
    public static PublicReleaseNoteDto ToPublicDto(this ReleaseNote note, IReadOnlySet<ReleaseNoteCategory>? categories)
    {
        var sections = note.Sections
            .Where(s => categories is null || categories.Contains(s.Category))
            .Select(s => new PublicReleaseNoteSectionDto(s.Category, s.Category.Label(), s.ContentHtml))
            .ToList();

        return new PublicReleaseNoteDto(
            note.Version,
            note.IsPreRelease,
            note.Title,
            note.SummaryHtml,
            sections,
            ComposeHtml(note.SummaryHtml, sections),
            note.PublishedAt!.Value);
    }

    private static string ComposeHtml(string summaryHtml, IEnumerable<PublicReleaseNoteSectionDto> sections)
    {
        var html = new StringBuilder(summaryHtml);
        foreach (var section in sections)
        {
            var category = section.Category.ToString().ToLowerInvariant();
            html.Append("<section data-category=\"").Append(category).Append("\">")
                .Append("<h2>").Append(WebUtility.HtmlEncode(section.Label)).Append("</h2>")
                .Append(section.ContentHtml)
                .Append("</section>");
        }

        return html.ToString();
    }
}
