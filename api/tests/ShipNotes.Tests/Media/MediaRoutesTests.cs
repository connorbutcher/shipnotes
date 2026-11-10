using FluentAssertions;
using ShipNotes.Application.Media;

namespace ShipNotes.Tests.Media;

public sealed class MediaRoutesTests
{
    private static readonly Guid Id = Guid.Parse("01933b5e-0000-7000-8000-000000000001");

    [Theory]
    [InlineData("https://notes.example.com/api/v1/media/01933b5e-0000-7000-8000-000000000001/shot.png")]
    [InlineData("http://localhost:5078/api/v1/media/01933b5e-0000-7000-8000-000000000001")]
    [InlineData("/api/v1/media/01933b5e-0000-7000-8000-000000000001/shot.png")]
    public void Reads_the_id_from_media_urls(string url)
    {
        MediaRoutes.TryGetMediaId(url, out var id).Should().BeTrue();
        id.Should().Be(Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("data:image/png;base64,iVBORw0KGgo=")]
    [InlineData("https://example.com/image.png")]
    [InlineData("https://example.com/proxy?u=/api/v1/media/01933b5e-0000-7000-8000-000000000001")]
    [InlineData("javascript:/api/v1/media/01933b5e-0000-7000-8000-000000000001")]
    public void Rejects_other_urls(string url) =>
        MediaRoutes.TryGetMediaId(url, out _).Should().BeFalse();

    [Fact]
    public void Finds_every_media_id_in_html() =>
        MediaRoutes.FindMediaIds($"<img src=\"/api/v1/media/{Id}/a.png\"><img src=\"https://x/api/v1/media/{Id}\">")
            .Should().Equal(Id);
}
