using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ShipNotes.Application.Abstractions;
using ShipNotes.Domain.Applications;

namespace ShipNotes.Infrastructure.Security;

/// <summary>
/// Generates high-entropy keys ("sn_" + 256 random bits) and stores only their SHA-256 hash.
/// A fast hash is appropriate because the keys are random, not user-chosen passwords.
/// </summary>
internal sealed class ApiKeyService : IApiKeyService
{
    private const string KeyPrefix = "sn_";
    private const int KeyBytes = 32;

    public GeneratedApiKey Generate()
    {
        var value = KeyPrefix + Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(KeyBytes));
        var credential = new ApiKeyCredential(Hash(value), value[..ApplicationConstraints.ApiKeyPrefixLength]);
        return new GeneratedApiKey(value, credential);
    }

    public string Hash(string apiKey) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey)));
}
