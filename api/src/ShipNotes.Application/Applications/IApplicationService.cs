namespace ShipNotes.Application.Applications;

public interface IApplicationService
{
    Task<IReadOnlyList<ApplicationDto>> ListAsync(CancellationToken cancellationToken);

    Task<ApplicationDto> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ApplicationWithApiKeyDto> CreateAsync(SaveApplicationRequest request, CancellationToken cancellationToken);

    Task<ApplicationDto> UpdateAsync(Guid id, SaveApplicationRequest request, CancellationToken cancellationToken);

    Task<ApplicationWithApiKeyDto> RegenerateApiKeyAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
