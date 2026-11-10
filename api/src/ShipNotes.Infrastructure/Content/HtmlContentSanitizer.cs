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

    public string Sanitize(string html) => sanitizer.Sanitize(html ?? string.Empty);

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
