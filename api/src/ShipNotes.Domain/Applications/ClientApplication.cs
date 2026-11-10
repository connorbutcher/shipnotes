using ShipNotes.Domain.Common;

namespace ShipNotes.Domain.Applications;

/// <summary>
/// An application that publishes release notes and consumes them through its API key.
/// Only a hash of the API key is stored; the raw key is shown to the user once.
/// </summary>
public sealed class ClientApplication : Entity
{
    private ClientApplication()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public string? IconUrl { get; private set; }

    public string ApiKeyHash { get; private set; } = string.Empty;

    public string ApiKeyPrefix { get; private set; } = string.Empty;

    public static ClientApplication Create(string name, string? iconUrl, ApiKeyCredential credential, DateTimeOffset now) =>
        Create(Guid.CreateVersion7(), name, iconUrl, credential, now);

    /// <summary>Creates an application with a known id, e.g. the seeded showcase demo.</summary>
    public static ClientApplication Create(Guid id, string name, string? iconUrl, ApiKeyCredential credential, DateTimeOffset now)
    {
        var application = new ClientApplication { Id = id };
        application.Update(name, iconUrl, now);
        application.SetApiKey(credential, now);
        return application;
    }

    public void Update(string name, string? iconUrl, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Application name is required.");
        }

        Name = name.Trim();
        IconUrl = string.IsNullOrWhiteSpace(iconUrl) ? null : iconUrl.Trim();
        Touch(now);
    }

    public void SetApiKey(ApiKeyCredential credential, DateTimeOffset now)
    {
        ApiKeyHash = credential.Hash;
        ApiKeyPrefix = credential.Prefix;
        Touch(now);
    }
}
