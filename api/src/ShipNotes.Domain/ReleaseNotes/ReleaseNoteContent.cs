namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>The written content of a release note: an optional summary and one section per category.</summary>
public sealed record ReleaseNoteContent(string SummaryHtml, IReadOnlyList<ReleaseNoteSectionContent> Sections);

public sealed record ReleaseNoteSectionContent(ReleaseNoteCategory Category, string ContentHtml);
