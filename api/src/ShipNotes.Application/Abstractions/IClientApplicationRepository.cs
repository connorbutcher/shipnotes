using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Abstractions;

public interface IClientApplicationRepository
{
    Task<IReadOnlyList<ClientApplication>> ListAsync(CancellationToken cancellationToken);

    Task<ClientApplication?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<ClientApplication?> FindByApiKeyHashAsync(string apiKeyHash, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, Guid? excludingId, CancellationToken cancellationToken);

    void Add(ClientApplication application);

    void Remove(ClientApplication application);
}
