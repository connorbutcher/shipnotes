using ShipNotes.Domain.Common;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>Release notes for one semantic version of a client application.</summary>
public sealed class ReleaseNote : Entity
{
    private readonly List<ReleaseNoteSection> sections = [];

    private ReleaseNote()
    {
    }

    public Guid ApplicationId { get; private set; }

    public string Version { get; private set; } = string.Empty;

    public string VersionSortKey { get; private set; } = string.Empty;

    public bool IsPreRelease { get; private set; }

    public string Title { get; private set; } = string.Empty;

    /// <summary>Introductory text shown before the categorised sections, e.g. highlights and screenshots.</summary>
    public string SummaryHtml { get; private set; } = string.Empty;

    /// <summary>The note's sections, at most one per category, in display order.</summary>
    public IReadOnlyList<ReleaseNoteSection> Sections => [.. sections.OrderBy(s => s.Category)];

    public IEnumerable<ReleaseNoteCategory> Categories => Sections.Select(s => s.Category);

    public DateTimeOffset? PublishedAt { get; private set; }

    public bool IsPublished => PublishedAt.HasValue;

    public static ReleaseNote Create(
        Guid applicationId,
        SemanticVersion version,
        string title,
        ReleaseNoteContent content,
        DateTimeOffset now)
    {
        var note = new ReleaseNote { ApplicationId = applicationId };
        note.Update(version, title, content, now);
        return note;
    }

    public void Update(SemanticVersion version, string title, ReleaseNoteContent content, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Release note title is required.");
        }

        var duplicate = content.Sections.GroupBy(s => s.Category).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
        {
            throw new DomainException($"A release note can only have one {duplicate.Key.Label()} section.");
        }

        Version = version.ToString();
        VersionSortKey = version.SortKey;
        IsPreRelease = version.IsPreRelease;
        Title = title.Trim();
        SummaryHtml = content.SummaryHtml;
        SetSections(content.Sections);
        Touch(now);
    }

    public void Publish(DateTimeOffset now)
    {
        PublishedAt ??= now;
        Touch(now);
    }

    public void Unpublish(DateTimeOffset now)
    {
        PublishedAt = null;
        Touch(now);
    }

    /// <summary>Updates sections in place so each category keeps its identity across edits.</summary>
    private void SetSections(IReadOnlyList<ReleaseNoteSectionContent> updated)
    {
        sections.RemoveAll(existing => updated.All(u => u.Category != existing.Category));

        foreach (var section in updated)
        {
            var existing = sections.Find(s => s.Category == section.Category);
            if (existing is null)
            {
                sections.Add(new ReleaseNoteSection(section.Category, section.ContentHtml));
            }
            else
            {
                existing.UpdateContent(section.ContentHtml);
            }
        }
    }
}
