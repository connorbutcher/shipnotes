using Microsoft.EntityFrameworkCore;
using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.Applications;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Infrastructure.Persistence;

public sealed class ShipNotesDbContext(DbContextOptions<ShipNotesDbContext> options)
    : DbContext(options), IUnitOfWork
{
    private const string SqlServerBinaryCollation = "Latin1_General_100_BIN2";

    public DbSet<ClientApplication> Applications => Set<ClientApplication>();

    public DbSet<ReleaseNote> ReleaseNotes => Set<ReleaseNote>();

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShipNotesDbContext).Assembly);

        // Version sort keys must compare ordinally (byte by byte) for semver precedence to hold.
        if (Database.IsSqlServer())
        {
            modelBuilder.Entity<ReleaseNote>()
                .Property(n => n.VersionSortKey)
                .UseCollation(SqlServerBinaryCollation);
        }
    }
}
