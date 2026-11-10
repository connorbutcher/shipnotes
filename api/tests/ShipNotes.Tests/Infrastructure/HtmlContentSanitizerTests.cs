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

    [Theory]
    [InlineData("<p>Hi</p><script>alert(1)</script>", "<p>Hi</p>")]
    [InlineData("<p onclick=\"alert(1)\">Hi</p>", "<p>Hi</p>")]
    [InlineData("<a href=\"javascript:alert(1)\">x</a>", "<a>x</a>")]
    [InlineData("<iframe src=\"https://evil\"></iframe><p>ok</p>", "<p>ok</p>")]
    public void Removes_unsafe_markup(string input, string expected) =>
        sanitizer.Sanitize(input).Should().Be(expected);
}
