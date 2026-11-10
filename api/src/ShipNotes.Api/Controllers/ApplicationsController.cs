using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShipNotes.Api.Extensions;
using ShipNotes.Application.Applications;

namespace ShipNotes.Api.Controllers;

[ApiController]
[Route("api/v1/applications")]
[EnableCors(CorsPolicies.AdminUi)]
public sealed class ApplicationsController(IApplicationService applications) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ApplicationDto>> List(CancellationToken cancellationToken) =>
        applications.ListAsync(cancellationToken);

    [HttpGet("{id:guid}", Name = nameof(GetApplication))]
    public Task<ApplicationDto> GetApplication(Guid id, CancellationToken cancellationToken) =>
        applications.GetAsync(id, cancellationToken);

    [HttpPost]
    [ProducesResponseType<ApplicationWithApiKeyDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(SaveApplicationRequest request, CancellationToken cancellationToken)
    {
        var created = await applications.CreateAsync(request, cancellationToken);
        return CreatedAtRoute(nameof(GetApplication), new { id = created.Application.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public Task<ApplicationDto> Update(Guid id, SaveApplicationRequest request, CancellationToken cancellationToken) =>
        applications.UpdateAsync(id, request, cancellationToken);

    [HttpPost("{id:guid}/api-key")]
    public Task<ApplicationWithApiKeyDto> RegenerateApiKey(Guid id, CancellationToken cancellationToken) =>
        applications.RegenerateApiKeyAsync(id, cancellationToken);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await applications.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
