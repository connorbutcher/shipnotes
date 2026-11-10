using FluentValidation;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Applications;

internal sealed class ApplicationService(
    IClientApplicationRepository applications,
    IApiKeyService apiKeys,
    IUnitOfWork unitOfWork,
    IValidator<SaveApplicationRequest> validator,
    TimeProvider clock) : IApplicationService
{
    public async Task<IReadOnlyList<ApplicationDto>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await applications.ListAsync(cancellationToken);
        return [.. items.Select(a => a.ToDto())];
    }

    public async Task<ApplicationDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        (await GetEntityAsync(id, cancellationToken)).ToDto();

    public async Task<ApplicationWithApiKeyDto> CreateAsync(SaveApplicationRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureNameIsUniqueAsync(request.Name, null, cancellationToken);

        var key = apiKeys.Generate();
        var application = ClientApplication.Create(request.Name, request.IconUrl, key.Credential, clock.GetUtcNow());

        applications.Add(application);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApplicationWithApiKeyDto(application.ToDto(), key.Value);
    }

    public async Task<ApplicationDto> UpdateAsync(Guid id, SaveApplicationRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var application = await GetEntityAsync(id, cancellationToken);
        await EnsureNameIsUniqueAsync(request.Name, id, cancellationToken);

        application.Update(request.Name, request.IconUrl, clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return application.ToDto();
    }

    public async Task<ApplicationWithApiKeyDto> RegenerateApiKeyAsync(Guid id, CancellationToken cancellationToken)
    {
        var application = await GetEntityAsync(id, cancellationToken);
        var key = apiKeys.Generate();

        application.SetApiKey(key.Credential, clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApplicationWithApiKeyDto(application.ToDto(), key.Value);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var application = await GetEntityAsync(id, cancellationToken);
        applications.Remove(application);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ClientApplication> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await applications.FindAsync(id, cancellationToken)
        ?? throw new NotFoundException("Application", id);

    private async Task EnsureNameIsUniqueAsync(string name, Guid? excludingId, CancellationToken cancellationToken)
    {
        if (await applications.NameExistsAsync(name.Trim(), excludingId, cancellationToken))
        {
            throw new ConflictException($"An application named '{name.Trim()}' already exists.");
        }
    }
}
