using ShipNotes.Domain.Common;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>Release notes for one semantic version of a client application.</summary>
public sealed class ReleaseNote : Entity
{
    private ReleaseNote()
    {
    }

    public Guid ApplicationId { get; private set; }

    public string Version { get; private set; } = string.Empty;

    public string VersionSortKey { get; private set; } = string.Empty;

    public bool IsPreRelease { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string ContentHtml { get; private set; } = string.Empty;

    public DateTimeOffset? PublishedAt { get; private set; }

    public bool IsPublished => PublishedAt.HasValue;

    public static ReleaseNote Create(
        Guid applicationId,
        SemanticVersion version,
        string title,
        string contentHtml,
        DateTimeOffset now)
    {
        var note = new ReleaseNote { ApplicationId = applicationId };
        note.Update(version, title, contentHtml, now);
        return note;
    }

    public void Update(SemanticVersion version, string title, string contentHtml, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Release note title is required.");
        }

        Version = version.ToString();
        VersionSortKey = version.SortKey;
        IsPreRelease = version.IsPreRelease;
        Title = title.Trim();
        ContentHtml = contentHtml;
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
}
