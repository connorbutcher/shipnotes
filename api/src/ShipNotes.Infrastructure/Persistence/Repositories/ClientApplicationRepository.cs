using Microsoft.EntityFrameworkCore;
using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.Applications;

namespace ShipNotes.Infrastructure.Persistence.Repositories;

internal sealed class ClientApplicationRepository(ShipNotesDbContext db) : IClientApplicationRepository
{
    public async Task<IReadOnlyList<ClientApplication>> ListAsync(CancellationToken cancellationToken) =>
        await db.Applications.AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);

    public Task<ClientApplication?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.Applications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<ClientApplication?> FindByApiKeyHashAsync(string apiKeyHash, CancellationToken cancellationToken) =>
        db.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.ApiKeyHash == apiKeyHash, cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? excludingId, CancellationToken cancellationToken) =>
        db.Applications.AnyAsync(a => a.Name == name && a.Id != excludingId, cancellationToken);

    public void Add(ClientApplication application) => db.Applications.Add(application);

    public void Remove(ClientApplication application) => db.Applications.Remove(application);
}
