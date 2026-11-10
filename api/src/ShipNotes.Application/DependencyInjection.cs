using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShipNotes.Application.Applications;
using ShipNotes.Application.Media;
using ShipNotes.Application.PublicApi;
using ShipNotes.Application.ReleaseNotes;
using ShipNotes.Application.Showcase;

namespace ShipNotes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IReleaseNoteService, ReleaseNoteService>();
        services.AddScoped<IPublicReleaseNoteService, PublicReleaseNoteService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IReleaseNoteMediaService, ReleaseNoteMediaService>();
        services.AddScoped<IShowcaseService, ShowcaseService>();

        return services;
    }
}
