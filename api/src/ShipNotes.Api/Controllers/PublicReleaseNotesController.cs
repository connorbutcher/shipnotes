using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShipNotes.Api.Authorization;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.PublicApi;

namespace ShipNotes.Api.Controllers;

/// <summary>
/// Read-only API for consuming applications. Every request must send the application's API key
/// in <c>X-Api-Key</c>, and the key must belong to the application in the route: a missing or
/// unknown key gets 401, a key for a different application gets 403.
/// </summary>
[ApiController]
[Route("api/v1/public/applications/{applicationId:guid}")]
[Authorize(Policy = AuthorizationPolicies.ApplicationApiKey)]
[EnableCors(CorsPolicies.PublicApi)]
[EnableRateLimiting(RateLimitPolicies.PublicApi)]
public sealed class PublicReleaseNotesController(IPublicReleaseNoteService releaseNotes) : ControllerBase
{
    [HttpGet]
    public Task<PublicApplicationDto> GetApplication(Guid applicationId, CancellationToken cancellationToken) =>
        releaseNotes.GetApplicationAsync(applicationId, cancellationToken);

    [HttpGet("release-notes")]
    public Task<IReadOnlyList<PublicReleaseNoteDto>> List(
        Guid applicationId,
        [FromQuery] PublicReleaseNotesQuery query,
        CancellationToken cancellationToken) =>
        releaseNotes.ListAsync(applicationId, query, cancellationToken);

    /// <summary>Published notes between two versions: after <c>from</c> (or from it, with <c>includeFrom</c>) up to and including <c>to</c>.</summary>
    [HttpGet("release-notes/between")]
    public Task<IReadOnlyList<PublicReleaseNoteDto>> ListBetween(
        Guid applicationId,
        [FromQuery] PublicReleaseNotesRangeQuery query,
        CancellationToken cancellationToken) =>
        releaseNotes.ListBetweenAsync(applicationId, query, cancellationToken);

    [HttpGet("release-notes/latest")]
    public Task<PublicReleaseNoteDto> GetLatest(
        Guid applicationId,
        [FromQuery] bool includePreReleases,
        CancellationToken cancellationToken) =>
        releaseNotes.GetLatestAsync(applicationId, includePreReleases, cancellationToken);

    [HttpGet("release-notes/{version}")]
    public Task<PublicReleaseNoteDto> GetByVersion(Guid applicationId, string version, CancellationToken cancellationToken) =>
        releaseNotes.GetByVersionAsync(applicationId, version, cancellationToken);
}
