using System.Text.RegularExpressions;

namespace ShipNotes.Application.Common;

/// <summary>Checks on sanitised rich text HTML.</summary>
internal static partial class HtmlText
{
    /// <summary>
    /// True when the HTML shows nothing: no text and no images, e.g. the editor's empty <c>&lt;p&gt;&lt;br&gt;&lt;/p&gt;</c>.
    /// </summary>
    public static bool IsBlank(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return true;
        }

        if (html.Contains("<img", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var text = Tags().Replace(html, string.Empty).Replace("&nbsp;", " ", StringComparison.Ordinal);
        return string.IsNullOrWhiteSpace(text);
    }

    [GeneratedRegex("<[^>]*>", RegexOptions.CultureInvariant)]
    private static partial Regex Tags();
}
