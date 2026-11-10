using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.Showcase;

namespace ShipNotes.Api.Controllers;

/// <summary>
/// Sets up the demo application used by the UI's showcase. Returns 404 unless Showcase:Enabled is true.
/// Each call issues a new API key for the demo application, so only enable it where that is acceptable.
/// </summary>
[ApiController]
[Route("api/v1/showcase")]
[EnableCors(CorsPolicies.AdminUi)]
public sealed class ShowcaseController(IShowcaseService showcase) : ControllerBase
{
    /// <summary>Seeds the demo application on first use and returns it with a new API key.</summary>
    [HttpPost("session")]
    public Task<ShowcaseSessionDto> Start(CancellationToken cancellationToken) =>
        showcase.StartAsync(cancellationToken);

    /// <summary>Discards any changes to the demo application and seeds it again.</summary>
    [HttpPost("reset")]
    public Task<ShowcaseSessionDto> Reset(CancellationToken cancellationToken) =>
        showcase.ResetAsync(cancellationToken);
}
