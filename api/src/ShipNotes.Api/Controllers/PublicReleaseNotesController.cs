using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShipNotes.Api.Authentication;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.PublicApi;

namespace ShipNotes.Api.Controllers;

/// <summary>Read-only API for consuming applications, authenticated with the application's API key.</summary>
[ApiController]
[Route("api/v1/public")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.Scheme)]
[EnableCors(CorsPolicies.PublicApi)]
[EnableRateLimiting(RateLimitPolicies.PublicApi)]
public sealed class PublicReleaseNotesController(IPublicReleaseNoteService releaseNotes) : ControllerBase
{
    [HttpGet("application")]
    public Task<PublicApplicationDto> GetApplication(CancellationToken cancellationToken) =>
        releaseNotes.GetApplicationAsync(User.GetApplicationId(), cancellationToken);

    [HttpGet("release-notes")]
    public Task<IReadOnlyList<PublicReleaseNoteDto>> List(
        [FromQuery] PublicReleaseNotesQuery query,
        CancellationToken cancellationToken) =>
        releaseNotes.ListAsync(User.GetApplicationId(), query, cancellationToken);

    [HttpGet("release-notes/latest")]
    public Task<PublicReleaseNoteDto> GetLatest(
        [FromQuery] bool includePreReleases,
        CancellationToken cancellationToken) =>
        releaseNotes.GetLatestAsync(User.GetApplicationId(), includePreReleases, cancellationToken);

    [HttpGet("release-notes/{version}")]
    public Task<PublicReleaseNoteDto> GetByVersion(string version, CancellationToken cancellationToken) =>
        releaseNotes.GetByVersionAsync(User.GetApplicationId(), version, cancellationToken);
}
