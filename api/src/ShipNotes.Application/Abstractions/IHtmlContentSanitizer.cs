namespace ShipNotes.Application.Abstractions;

/// <summary>Removes unsafe markup so release note HTML can be rendered by consuming apps.</summary>
public interface IHtmlContentSanitizer
{
    string Sanitize(string html);
}
