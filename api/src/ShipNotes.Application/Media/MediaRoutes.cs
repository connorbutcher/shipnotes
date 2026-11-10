using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ShipNotes.Application.Media;

/// <summary>The public media URL shape (<c>/api/v1/media/{id}/{fileName}</c>) and how to read ids back out of it.</summary>
public static partial class MediaRoutes
{
    public const string BasePath = "api/v1/media";

    public static string GetPath(Guid id, string fileName) =>
        $"/{BasePath}/{id}/{Uri.EscapeDataString(fileName)}";

    /// <summary>Reads the media id from an absolute or relative media URL.</summary>
    public static bool TryGetMediaId(string? url, [NotNullWhen(true)] out Guid? id)
    {
        id = null;
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        var match = MediaUrlRegex().Match(url.Trim());
        if (match.Success && Guid.TryParse(match.Groups["id"].Value, out var parsed))
        {
            id = parsed;
            return true;
        }

        return false;
    }

    /// <summary>Ids of every media URL that appears anywhere in the HTML.</summary>
    public static IReadOnlySet<Guid> FindMediaIds(string html)
    {
        var ids = new HashSet<Guid>();
        foreach (Match match in MediaIdInHtmlRegex().Matches(html ?? string.Empty))
        {
            if (Guid.TryParse(match.Groups["id"].Value, out var id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    [GeneratedRegex(@"^(?:https?://[^/?#]+)?/api/v1/media/(?<id>[0-9a-fA-F]{8}-(?:[0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12})(?:/[^/?#]*)?$", RegexOptions.CultureInvariant)]
    private static partial Regex MediaUrlRegex();

    [GeneratedRegex(@"/api/v1/media/(?<id>[0-9a-fA-F]{8}-(?:[0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12})", RegexOptions.CultureInvariant)]
    private static partial Regex MediaIdInHtmlRegex();
}
