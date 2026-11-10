using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.Media;

namespace ShipNotes.Api.Controllers;

/// <summary>Uploads images for the release note editor. The returned URL is what goes into the note HTML.</summary>
[ApiController]
[Route("api/v1/applications/{applicationId:guid}/media")]
[EnableCors(CorsPolicies.AdminUi)]
public sealed class MediaUploadController(IMediaService media) : ControllerBase
{
    /// <summary>Multipart limit; the configured Media:MaxFileSizeBytes is enforced by the service.</summary>
    private const long RequestSizeLimitBytes = 25 * 1024 * 1024;

    [HttpPost]
    [RequestSizeLimit(RequestSizeLimitBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = RequestSizeLimitBytes)]
    [ProducesResponseType<MediaDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Upload(Guid applicationId, IFormFile file, CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();
        var uploaded = await media.UploadAsync(
            applicationId,
            new UploadMediaRequest(file.FileName, file.Length, content),
            cancellationToken);

        return Created(uploaded.Url, uploaded);
    }
}
