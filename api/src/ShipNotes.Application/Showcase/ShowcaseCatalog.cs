namespace ShipNotes.Application.Showcase;

/// <summary>A release note seeded into the demo application. <see cref="ContentFile"/> names an embedded HTML file.</summary>
internal sealed record ShowcaseNote(string Version, string Title, string ContentFile, bool IsPublished = true);

/// <summary>The demo application's content: a fictional task app with a realistic release history.</summary>
internal static class ShowcaseCatalog
{
    public const string IconFile = "orbit-icon.png";

    /// <summary>
    /// Covers every query the public API supports: stable and pre-release versions, a patch release,
    /// a major version, notes with and without images, and an unpublished draft that is never returned.
    /// </summary>
    public static readonly IReadOnlyList<ShowcaseNote> Notes =
    [
        new("1.0.0", "Welcome to Orbit Tasks", "1.0.0.html"),
        new("1.1.0", "Kanban boards", "1.1.0.html"),
        new("1.1.1", "Bug fixes", "1.1.1.html"),
        new("1.2.0", "Dark mode", "1.2.0.html"),
        new("1.3.0-beta.1", "Smart search preview", "1.3.0-beta.1.html"),
        new("1.3.0", "Smart search", "1.3.0.html"),
        new("2.0.0-rc.1", "Calendar view release candidate", "2.0.0-rc.1.html"),
        new("2.0.0", "Calendar view", "2.0.0.html"),
        new("2.1.0", "Recurring tasks (draft)", "2.1.0.html", IsPublished: false),
    ];
}
