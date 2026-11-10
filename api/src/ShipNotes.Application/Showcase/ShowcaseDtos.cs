using ShipNotes.Application.Applications;

namespace ShipNotes.Application.Showcase;

/// <summary>
/// The demo application and a freshly issued API key for it. Each session rotates the key,
/// so the raw key is still only ever shown in the response that created it.
/// </summary>
public sealed record ShowcaseSessionDto(ApplicationDto Application, string ApiKey);
