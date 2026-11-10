using AngleSharp.Html.Parser;
using Ganss.Xss;
using ShipNotes.Application.Abstractions;

namespace ShipNotes.Infrastructure.Content;

/// <summary>Allow-list sanitiser for rich text produced by the editor.</summary>
internal sealed class HtmlContentSanitizer : IHtmlContentSanitizer
{
    private static readonly string[] AllowedTags =
    [
        "p", "br", "h1", "h2", "h3", "h4", "strong", "b", "em", "i", "u", "s",
        "blockquote", "pre", "code", "ol", "ul", "li", "a", "img", "span",
    ];

    private static readonly string[] AllowedAttributes = ["href", "src", "alt", "title", "target", "rel", "class", "data-list"];

    private readonly HtmlSanitizer sanitizer = CreateSanitizer();

    public string Sanitize(string html, Func<string, string?>? resolveImageSource = null)
    {
        var clean = sanitizer.Sanitize(html ?? string.Empty);
        return clean.Contains("<img", StringComparison.OrdinalIgnoreCase)
            ? ResolveImages(clean, resolveImageSource)
            : clean;
    }

    /// <summary>
    /// Keeps only images whose source the caller recognises (uploaded media), so inline data URIs
    /// and hot-linked images never reach consuming apps.
    /// </summary>
    private static string ResolveImages(string html, Func<string, string?>? resolveImageSource)
    {
        var document = new HtmlParser().ParseDocument(html);
        var body = document.Body!;

        foreach (var image in body.QuerySelectorAll("img").ToList())
        {
            var source = resolveImageSource?.Invoke(image.GetAttribute("src") ?? string.Empty);
            if (source is null)
            {
                image.Remove();
            }
            else
            {
                image.SetAttribute("src", source);
            }
        }

        return body.InnerHtml;
    }

    private static HtmlSanitizer CreateSanitizer()
    {
        var options = new HtmlSanitizerOptions
        {
            AllowedTags = new HashSet<string>(AllowedTags, StringComparer.OrdinalIgnoreCase),
            AllowedAttributes = new HashSet<string>(AllowedAttributes, StringComparer.OrdinalIgnoreCase),
            AllowedSchemes = new HashSet<string>(["http", "https", "mailto"], StringComparer.OrdinalIgnoreCase),
            UriAttributes = new HashSet<string>(["href", "src"], StringComparer.OrdinalIgnoreCase),
        };

        return new HtmlSanitizer(options);
    }
}
