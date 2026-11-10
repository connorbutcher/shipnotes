using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipNotes.Application.Abstractions;
using ShipNotes.Infrastructure.Content;
using ShipNotes.Infrastructure.Persistence;
using ShipNotes.Infrastructure.Persistence.Repositories;
using ShipNotes.Infrastructure.Security;

namespace ShipNotes.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "ShipNotes";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<ShipNotesDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ShipNotesDbContext>());
        services.AddScoped<IClientApplicationRepository, ClientApplicationRepository>();
        services.AddScoped<IReleaseNoteRepository, ReleaseNoteRepository>();

        services.AddSingleton<IApiKeyService, ApiKeyService>();
        services.AddSingleton<IHtmlContentSanitizer, HtmlContentSanitizer>();

        return services;
    }
}
