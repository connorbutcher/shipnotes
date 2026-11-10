using FluentAssertions;
using ShipNotes.Application.Media;

namespace ShipNotes.Tests.Media;

public sealed class MediaFileTests
{
    [Theory]
    [InlineData("Screen Shot 2026-11-10 at 10.00.png", ".png", "screen-shot-2026-11-10-at-10-00.png")]
    [InlineData("../../etc/passwd", ".jpg", "passwd.jpg")]
    [InlineData("测试.gif", ".gif", "image.gif")]
    [InlineData(null, ".webp", "image.webp")]
    public void Normalises_uploaded_file_names(string? input, string extension, string expected) =>
        MediaFileName.Normalize(input, extension).Should().Be(expected);

    [Fact]
    public void Detects_image_types_from_content()
    {
        ImageFormat.Detect([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0])!.ContentType.Should().Be("image/png");
        ImageFormat.Detect([0xFF, 0xD8, 0xFF, 0xE0])!.ContentType.Should().Be("image/jpeg");
        ImageFormat.Detect("GIF89a......"u8)!.ContentType.Should().Be("image/gif");
        ImageFormat.Detect("RIFF\0\0\0\0WEBP"u8)!.ContentType.Should().Be("image/webp");
        ImageFormat.Detect("<svg xmlns=\""u8).Should().BeNull();
    }
}
