namespace ShipNotes.Application.Showcase;

/// <summary>
/// A release note seeded into the demo application. Its content is read from embedded HTML files named after the version:
/// <c>{version}.html</c> for the summary and <c>{version}.{category}.html</c> for each section (see <see cref="ShowcaseContent"/>).
/// </summary>
internal sealed record ShowcaseNote(string Version, string Title, bool IsPublished = true);

/// <summary>The demo application's content: a fictional task app with a realistic release history.</summary>
internal static class ShowcaseCatalog
{
    public const string IconFile = "orbit-icon.png";

    /// <summary>
    /// Covers every query the public API supports: stable and pre-release versions, a patch release,
    /// a major version, notes with and without images, every category of change,
    /// and an unpublished draft that is never returned.
    /// </summary>
    public static readonly IReadOnlyList<ShowcaseNote> Notes =
    [
        new("1.0.0", "Welcome to Orbit Tasks"),
        new("1.1.0", "Kanban boards"),
        new("1.1.1", "Bug fixes"),
        new("1.2.0", "Dark mode"),
        new("1.3.0-beta.1", "Smart search preview"),
        new("1.3.0", "Smart search"),
        new("2.0.0-rc.1", "Calendar view release candidate"),
        new("2.0.0", "Calendar view"),
        new("2.1.0", "Recurring tasks (draft)", IsPublished: false),
    ];
}
