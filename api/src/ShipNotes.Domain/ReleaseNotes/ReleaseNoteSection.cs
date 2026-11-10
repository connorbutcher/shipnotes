namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>The rich text for one category of change within a release note.</summary>
public sealed class ReleaseNoteSection
{
    private ReleaseNoteSection()
    {
    }

    internal ReleaseNoteSection(ReleaseNoteCategory category, string contentHtml)
    {
        Category = category;
        ContentHtml = contentHtml;
    }

    public ReleaseNoteCategory Category { get; private set; }

    public string ContentHtml { get; private set; } = string.Empty;

    internal void UpdateContent(string contentHtml) => ContentHtml = contentHtml;
}
