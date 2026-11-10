namespace ShipNotes.Application.Media;

/// <summary>Media upload settings, bound from the "Media" configuration section.</summary>
public sealed class MediaOptions
{
    public const string SectionName = "Media";

    /// <summary>Folder that uploaded files are written to, e.g. F:\Dev\ShipNotesMedia.</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>
    /// Public base URL of this API used in media links, e.g. https://shipnotes.example.com.
    /// When empty, the scheme and host of the upload request are used.
    /// </summary>
    public string? PublicBaseUrl { get; set; }

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
}
