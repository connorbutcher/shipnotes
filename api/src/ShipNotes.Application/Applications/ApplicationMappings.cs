using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Applications;

internal static class ApplicationMappings
{
    public static ApplicationDto ToDto(this ClientApplication application) =>
        new(
            application.Id,
            application.Name,
            application.IconUrl,
            application.ApiKeyPrefix,
            application.CreatedAt,
            application.UpdatedAt);
}
