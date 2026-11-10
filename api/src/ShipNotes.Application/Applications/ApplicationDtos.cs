namespace ShipNotes.Application.Applications;

public sealed record ApplicationDto(
    Guid Id,
    string Name,
    string? IconUrl,
    string ApiKeyPrefix,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>Returned only when a key is created or regenerated; the raw key is never retrievable again.</summary>
public sealed record ApplicationWithApiKeyDto(ApplicationDto Application, string ApiKey);

public sealed record SaveApplicationRequest(string Name, string? IconUrl);
