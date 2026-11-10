using FluentAssertions;
using ShipNotes.Domain.Applications;
using ShipNotes.Infrastructure.Security;

namespace ShipNotes.Tests.Infrastructure;

public sealed class ApiKeyServiceTests
{
    private readonly ApiKeyService service = new();

    [Fact]
    public void Generate_creates_unique_prefixed_keys_with_matching_hashes()
    {
        var first = service.Generate();
        var second = service.Generate();

        first.Value.Should().StartWith("sn_").And.NotBe(second.Value);
        first.Credential.Hash.Should().HaveLength(ApplicationConstraints.ApiKeyHashLength).And.Be(service.Hash(first.Value));
        first.Credential.Prefix.Should().HaveLength(ApplicationConstraints.ApiKeyPrefixLength);
        first.Value.Should().StartWith(first.Credential.Prefix);
    }
}
