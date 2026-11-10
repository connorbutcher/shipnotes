namespace ShipNotes.Application.Showcase;

public interface IShowcaseService
{
    /// <summary>Seeds the demo application if it doesn't exist yet, then issues it a new API key.</summary>
    Task<ShowcaseSessionDto> StartAsync(CancellationToken cancellationToken);

    /// <summary>Deletes the demo application, including any edits made to it, and seeds it again.</summary>
    Task<ShowcaseSessionDto> ResetAsync(CancellationToken cancellationToken);
}
