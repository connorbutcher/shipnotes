using FluentAssertions;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Tests.Domain;

public sealed class SemanticVersionTests
{
    [Theory]
    [InlineData("1.0.0")]
    [InlineData("0.1.2")]
    [InlineData("10.20.30")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0-alpha.1")]
    [InlineData("1.0.0-0.3.7")]
    [InlineData("1.0.0-x.7.z.92")]
    [InlineData("1.0.0+20130313144700")]
    [InlineData("1.0.0-beta+exp.sha.5114f85")]
    public void TryParse_accepts_valid_versions(string value)
    {
        SemanticVersion.TryParse(value, out var version).Should().BeTrue();
        version!.ToString().Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("1.0")]
    [InlineData("01.0.0")]
    [InlineData("1.0.0-")]
    [InlineData("1.0.0-01")]
    [InlineData("v1.0.0")]
    [InlineData("1.0.0.0")]
    public void TryParse_rejects_invalid_versions(string value) =>
        SemanticVersion.TryParse(value, out _).Should().BeFalse();

    [Fact]
    public void Ordering_follows_the_semver_precedence_example()
    {
        // From https://semver.org/#spec-item-11
        string[] ascending =
        [
            "1.0.0-alpha", "1.0.0-alpha.1", "1.0.0-alpha.beta", "1.0.0-beta", "1.0.0-beta.2",
            "1.0.0-beta.11", "1.0.0-rc.1", "1.0.0", "1.0.1", "1.2.0", "1.10.0", "2.0.0",
        ];

        var shuffled = ascending.Reverse().Select(SemanticVersion.Parse).ToList();
        shuffled.Sort();

        shuffled.Select(v => v.ToString()).Should().Equal(ascending);
    }

    [Fact]
    public void Ordering_compares_identifier_by_identifier()
    {
        // "alpha" < "alpha-x" as identifiers, so "alpha.x" (first identifier "alpha") sorts first.
        SemanticVersion.Parse("1.0.0-alpha").Should().BeLessThan(SemanticVersion.Parse("1.0.0-alpha.x"));
        SemanticVersion.Parse("1.0.0-alpha.x").Should().BeLessThan(SemanticVersion.Parse("1.0.0-alpha-x"));
    }

    [Fact]
    public void Build_metadata_does_not_affect_precedence() =>
        SemanticVersion.Parse("1.0.0+build.1").SortKey.Should().Be(SemanticVersion.Parse("1.0.0+build.2").SortKey);
}
