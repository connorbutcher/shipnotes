using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShipNotes.Infrastructure.Persistence;

namespace ShipNotes.Tests.Integration;

/// <summary>Hosts the API in memory with a private SQLite database per factory instance.</summary>
public sealed class ShipNotesApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:ShipNotes", "unused");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<ShipNotesDbContext>>();
            services.RemoveAll<DbContextOptions<ShipNotesDbContext>>();

            connection.Open();
            services.AddDbContext<ShipNotesDbContext>(options => options.UseSqlite(connection));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ShipNotesDbContext>().Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            connection.Dispose();
        }
    }
}
