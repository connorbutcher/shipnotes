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

    /// <summary>Private media folder for this factory, deleted on dispose.</summary>
    public string MediaRoot { get; } = Path.Combine(Path.GetTempPath(), "shipnotes-tests", Guid.NewGuid().ToString("N"));

    /// <summary>Turns on the showcase endpoints, which are off outside Development.</summary>
    public bool ShowcaseEnabled { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:ShipNotes", "unused");
        builder.UseSetting("Media:RootPath", MediaRoot);
        builder.UseSetting("Showcase:Enabled", ShowcaseEnabled.ToString());

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
            if (Directory.Exists(MediaRoot))
            {
                Directory.Delete(MediaRoot, recursive: true);
            }
        }
    }
}
