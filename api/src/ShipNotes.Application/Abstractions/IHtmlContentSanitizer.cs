namespace ShipNotes.Application.Abstractions;

/// <summary>Removes unsafe markup so release note HTML can be rendered by consuming apps.</summary>
public interface IHtmlContentSanitizer
{
    /// <summary>Sanitises HTML.</summary>
    /// <param name="html">The HTML to clean.</param>
    /// <param name="resolveImageSource">
    /// Maps each image <c>src</c> to the URL to keep, or null to remove the image.
    /// When omitted, every image is removed.
    /// </param>
    string Sanitize(string html, Func<string, string?>? resolveImageSource = null);
}
