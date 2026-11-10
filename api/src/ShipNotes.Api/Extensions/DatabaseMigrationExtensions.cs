using Microsoft.EntityFrameworkCore;
using ShipNotes.Infrastructure.Persistence;

namespace ShipNotes.Api.Extensions;

internal static class DatabaseMigrationExtensions
{
    /// <summary>Applies pending EF Core migrations; intended for local development only.</summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ShipNotesDbContext>();
        await db.Database.MigrateAsync();
    }
}
