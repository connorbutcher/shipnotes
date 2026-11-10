namespace ShipNotes.Application.Showcase;

/// <summary>Showcase settings, bound from the "Showcase" configuration section.</summary>
public sealed class ShowcaseOptions
{
    public const string SectionName = "Showcase";

    /// <summary>
    /// Whether the showcase endpoints are available. Starting the showcase creates a demo application
    /// and issues it a new API key, so leave this off in production.
    /// </summary>
    public bool Enabled { get; set; }
}
