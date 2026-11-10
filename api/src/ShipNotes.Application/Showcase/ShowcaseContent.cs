using System.Reflection;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.Showcase;

/// <summary>Reads the demo notes and images that are embedded in this assembly (see Showcase/Content).</summary>
internal static class ShowcaseContent
{
    /// <summary>Must match the LogicalName prefix in ShipNotes.Application.csproj.</summary>
    private const string ResourcePrefix = "ShipNotes.Showcase.";

    private static readonly Assembly ResourceAssembly = typeof(ShowcaseContent).Assembly;

    /// <summary>Reads a note's summary (<c>{version}.html</c>) and its sections (<c>{version}.{category}.html</c>).</summary>
    public static ReleaseNoteContent ReadNote(string version)
    {
        var summary = TryRead($"{version}.html") ?? string.Empty;
        var sections = ReleaseNoteCategories.All
            .Select(category => (category, html: TryRead($"{version}.{category.ToString().ToLowerInvariant()}.html")))
            .Where(s => s.html is not null)
            .Select(s => new ReleaseNoteSectionContent(s.category, s.html!))
            .ToList();

        return new ReleaseNoteContent(summary, sections);
    }

    public static Stream OpenMedia(string fileName) => Open(fileName);

    private static string? TryRead(string fileName)
    {
        using var stream = ResourceAssembly.GetManifestResourceStream(ResourcePrefix + fileName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static Stream Open(string fileName) =>
        ResourceAssembly.GetManifestResourceStream(ResourcePrefix + fileName)
        ?? throw new InvalidOperationException($"Showcase content '{fileName}' is not embedded in {ResourceAssembly.GetName().Name}.");
}
