using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.ReleaseNotes;

namespace ShipNotes.Api.Controllers;

[ApiController]
[Route("api/v1/applications/{applicationId:guid}/release-notes")]
[EnableCors(CorsPolicies.AdminUi)]
public sealed class ReleaseNotesController(IReleaseNoteService releaseNotes) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ReleaseNoteSummaryDto>> List(Guid applicationId, CancellationToken cancellationToken) =>
        releaseNotes.ListAsync(applicationId, cancellationToken);

    [HttpGet("{id:guid}", Name = nameof(GetReleaseNote))]
    public Task<ReleaseNoteDto> GetReleaseNote(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        releaseNotes.GetAsync(applicationId, id, cancellationToken);

    [HttpPost]
    [ProducesResponseType<ReleaseNoteDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(Guid applicationId, SaveReleaseNoteRequest request, CancellationToken cancellationToken)
    {
        var created = await releaseNotes.CreateAsync(applicationId, request, cancellationToken);
        return CreatedAtRoute(nameof(GetReleaseNote), new { applicationId, id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public Task<ReleaseNoteDto> Update(Guid applicationId, Guid id, SaveReleaseNoteRequest request, CancellationToken cancellationToken) =>
        releaseNotes.UpdateAsync(applicationId, id, request, cancellationToken);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        await releaseNotes.DeleteAsync(applicationId, id, cancellationToken);
        return NoContent();
    }
}
