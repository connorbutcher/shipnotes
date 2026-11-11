using System.Reflection;

namespace ShipNotes.Application.Showcase;

/// <summary>Reads the demo notes and images that are embedded in this assembly (see Showcase/Content).</summary>
internal static class ShowcaseContent
{
    /// <summary>Must match the LogicalName prefix in ShipNotes.Application.csproj.</summary>
    private const string ResourcePrefix = "ShipNotes.Showcase.";

    private static readonly Assembly ResourceAssembly = typeof(ShowcaseContent).Assembly;

    public static string ReadNote(string fileName)
    {
        using var reader = new StreamReader(Open(fileName));
        return reader.ReadToEnd();
    }

    public static Stream OpenMedia(string fileName) => Open(fileName);

    private static Stream Open(string fileName) =>
        ResourceAssembly.GetManifestResourceStream(ResourcePrefix + fileName)
        ?? throw new InvalidOperationException($"Showcase content '{fileName}' is not embedded in {ResourceAssembly.GetName().Name}.");
}
