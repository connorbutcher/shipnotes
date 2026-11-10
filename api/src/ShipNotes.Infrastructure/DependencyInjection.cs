using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Media;
using ShipNotes.Infrastructure.Content;
using ShipNotes.Infrastructure.Media;
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
        services.AddScoped<IMediaRepository, MediaRepository>();

        services.AddSingleton<IApiKeyService, ApiKeyService>();
        services.AddSingleton<IHtmlContentSanitizer, HtmlContentSanitizer>();

        services.AddOptions<MediaOptions>()
            .Bind(configuration.GetSection(MediaOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.RootPath), "Media:RootPath must be configured.")
            .Validate(o => o.MaxFileSizeBytes > 0, "Media:MaxFileSizeBytes must be positive.")
            .Validate(
                o => string.IsNullOrWhiteSpace(o.PublicBaseUrl) || Uri.TryCreate(o.PublicBaseUrl, UriKind.Absolute, out _),
                "Media:PublicBaseUrl must be an absolute URL.")
            .ValidateOnStart();
        services.AddSingleton<IMediaStorage, FileSystemMediaStorage>();

        return services;
    }
}
