using FluentAssertions;
using ShipNotes.Infrastructure.Content;

namespace ShipNotes.Tests.Infrastructure;

public sealed class HtmlContentSanitizerTests
{
    private readonly HtmlContentSanitizer sanitizer = new();

    [Fact]
    public void Keeps_editor_formatting()
    {
        const string html = "<h2>New</h2><p><strong>Bold</strong> and <a href=\"https://example.com\">link</a></p><ul><li>One</li></ul>";

        sanitizer.Sanitize(html).Should().Be(html);
    }

    [Fact]
    public void Keeps_sections_and_their_classes_as_written()
    {
        const string html =
            "<p>Intro</p><section class=\"note-section note-section--fixed\"><h2 class=\"note-section__title\">Fixed</h2><p>Crash</p></section>";

        sanitizer.Sanitize(html).Should().Be(html);
    }

    [Theory]
    [InlineData("<p>Hi</p><script>alert(1)</script>", "<p>Hi</p>")]
    [InlineData("<p onclick=\"alert(1)\">Hi</p>", "<p>Hi</p>")]
    [InlineData("<a href=\"javascript:alert(1)\">x</a>", "<a>x</a>")]
    [InlineData("<iframe src=\"https://evil\"></iframe><p>ok</p>", "<p>ok</p>")]
    public void Removes_unsafe_markup(string input, string expected) =>
        sanitizer.Sanitize(input).Should().Be(expected);

    [Fact]
    public void Removes_images_unless_the_caller_resolves_their_source()
    {
        const string html = "<p><img src=\"https://example.com/a.png\" alt=\"A\"></p><p><img src=\"data:image/png;base64,AAAA\"></p>";

        sanitizer.Sanitize(html).Should().Be("<p></p><p></p>");
        sanitizer.Sanitize(html, src => src.StartsWith("https://", StringComparison.Ordinal) ? "https://cdn.test/a.png" : null)
            .Should().Be("<p><img src=\"https://cdn.test/a.png\" alt=\"A\"></p><p></p>");
    }
}
