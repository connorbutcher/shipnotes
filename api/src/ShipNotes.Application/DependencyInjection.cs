using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShipNotes.Application.Applications;
using ShipNotes.Application.PublicApi;
using ShipNotes.Application.ReleaseNotes;

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

        return services;
    }
}
