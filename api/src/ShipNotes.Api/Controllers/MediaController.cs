using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.Media;

namespace ShipNotes.Api.Controllers;

/// <summary>
/// Serves uploaded media to the editor and to consuming apps. Anonymous so a plain &lt;img src&gt; works;
/// ids are unguessable and a file never changes once uploaded, so it can be cached forever.
/// </summary>
[ApiController]
[Route(MediaRoutes.BasePath)]
[EnableCors(CorsPolicies.PublicApi)]
public sealed class MediaController(IMediaService media) : ControllerBase
{
    private const int CacheSeconds = 365 * 24 * 60 * 60;

    [HttpGet("{id:guid}/{fileName?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var file = await media.OpenAsync(id, cancellationToken);

        Response.Headers[HeaderNames.CacheControl] = $"public, max-age={CacheSeconds}, immutable";
        Response.Headers[HeaderNames.XContentTypeOptions] = "nosniff";
        Response.Headers[HeaderNames.ContentSecurityPolicy] = "default-src 'none'; sandbox";
        Response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";

        return File(file.Content, file.ContentType, enableRangeProcessing: true);
    }
}
