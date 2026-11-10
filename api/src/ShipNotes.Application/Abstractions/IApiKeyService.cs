using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Abstractions;

/// <summary>Creates API keys and derives the hashed form that is persisted.</summary>
public interface IApiKeyService
{
    GeneratedApiKey Generate();

    string Hash(string apiKey);
}

/// <summary>A newly generated key: the raw value (shown once) and its stored credential.</summary>
public sealed record GeneratedApiKey(string Value, ApiKeyCredential Credential);
