using FluentAssertions;
using ShipNotes.Domain.Common;
using ShipNotes.Domain.ReleaseNotes;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Tests.Domain;

public sealed class ReleaseNoteCategoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData("fixed", ReleaseNoteCategory.Fixed)]
    [InlineData("SECURITY", ReleaseNoteCategory.Security)]
    [InlineData(" Breaking ", ReleaseNoteCategory.Breaking)]
    public void Names_parse_ignoring_case(string value, ReleaseNoteCategory expected)
    {
        ReleaseNoteCategories.TryParse(value, out var category).Should().BeTrue();
        category.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("bugfix")]
    [InlineData("fixed,security")]
    public void Numbers_and_unknown_names_are_rejected(string? value) =>
        ReleaseNoteCategories.TryParse(value, out _).Should().BeFalse();

    [Fact]
    public void Breaking_changes_have_a_readable_label() =>
        ReleaseNoteCategory.Breaking.Label().Should().Be("Breaking changes");

    [Fact]
    public void Sections_are_kept_in_display_order()
    {
        var note = Create(
            new(ReleaseNoteCategory.Security, "<p>s</p>"),
            new(ReleaseNoteCategory.Added, "<p>a</p>"),
            new(ReleaseNoteCategory.Breaking, "<p>b</p>"));

        note.Categories.Should().Equal(ReleaseNoteCategory.Breaking, ReleaseNoteCategory.Added, ReleaseNoteCategory.Security);
    }

    [Fact]
    public void Updating_replaces_sections_and_keeps_matching_ones()
    {
        var note = Create(new(ReleaseNoteCategory.Added, "<p>old</p>"), new(ReleaseNoteCategory.Fixed, "<p>f</p>"));
        var added = note.Sections[0];

        note.Update(
            SemanticVersion.Parse("1.0.0"),
            "Title",
            new ReleaseNoteContent("", [new(ReleaseNoteCategory.Added, "<p>new</p>"), new(ReleaseNoteCategory.Security, "<p>s</p>")]),
            Now);

        note.Categories.Should().Equal(ReleaseNoteCategory.Added, ReleaseNoteCategory.Security);
        note.Sections[0].Should().BeSameAs(added);
        added.ContentHtml.Should().Be("<p>new</p>");
    }

    [Fact]
    public void A_category_can_only_appear_once()
    {
        var act = () => Create(new(ReleaseNoteCategory.Fixed, "<p>a</p>"), new(ReleaseNoteCategory.Fixed, "<p>b</p>"));

        act.Should().Throw<DomainException>().WithMessage("*one Fixed section*");
    }

    private static ReleaseNote Create(params ReleaseNoteSectionContent[] sections) =>
        ReleaseNote.Create(Guid.NewGuid(), SemanticVersion.Parse("1.0.0"), "Title", new ReleaseNoteContent("", sections), Now);
}
