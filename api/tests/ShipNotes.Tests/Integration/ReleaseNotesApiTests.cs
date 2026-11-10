using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.ReleaseNotes;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Tests.Integration;

public sealed class ReleaseNotesApiTests : IDisposable
{
    private readonly ShipNotesApiFactory factory = new();
    private readonly HttpClient client;

    public ReleaseNotesApiTests() => client = factory.CreateClient();

    [Fact]
    public async Task List_returns_notes_newest_version_first()
    {
        var app = await client.CreateApplicationAsync();
        foreach (var version in new[] { "1.2.0", "1.10.0", "1.10.0-beta.1", "1.9.3" })
        {
            await client.CreateReleaseNoteAsync(app.Application.Id, version);
        }

        var notes = await client.GetFromJsonAsync<List<ReleaseNoteSummaryDto>>($"/api/v1/applications/{app.Application.Id}/release-notes", TestJson.Options);

        notes!.Select(n => n.Version).Should().Equal("1.10.0", "1.10.0-beta.1", "1.9.3", "1.2.0");
    }

    [Fact]
    public async Task Create_sanitises_html_content()
    {
        var app = await client.CreateApplicationAsync();

        var note = await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: "<p>Hi</p><script>alert(1)</script>");

        note.SummaryHtml.Should().Be("<p>Hi</p>");
    }

    [Fact]
    public async Task Create_rejects_invalid_and_duplicate_versions()
    {
        var app = await client.CreateApplicationAsync();
        var url = $"/api/v1/applications/{app.Application.Id}/release-notes";
        await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0");

        var invalid = await client.PostAsJsonAsync(url, new SaveReleaseNoteRequest("1.0", "Bad", "<p></p>", [], false), TestJson.Options);
        var duplicate = await client.PostAsJsonAsync(url, new SaveReleaseNoteRequest("1.0.0+build.5", "Dup", "<p></p>", [], false), TestJson.Options);

        invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_changes_content_and_publish_state()
    {
        var app = await client.CreateApplicationAsync();
        var note = await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", isPublished: false);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/applications/{app.Application.Id}/release-notes/{note.Id}",
            new SaveReleaseNoteRequest("1.0.1", "Patched", "<p>Fixed</p>", [], true),
            TestJson.Options);
        var updated = (await response.Content.ReadFromJsonAsync<ReleaseNoteDto>(TestJson.Options))!;

        updated.Version.Should().Be("1.0.1");
        updated.Title.Should().Be("Patched");
        updated.IsPublished.Should().BeTrue();
        updated.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Notes_are_scoped_to_their_application()
    {
        var first = await client.CreateApplicationAsync("First");
        var second = await client.CreateApplicationAsync("Second");
        var note = await client.CreateReleaseNoteAsync(first.Application.Id, "1.0.0");

        var response = await client.GetAsync($"/api/v1/applications/{second.Application.Id}/release-notes/{note.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Sections_are_sanitised_ordered_and_blank_ones_dropped()
    {
        var app = await client.CreateApplicationAsync();

        var note = await client.CreateReleaseNoteAsync(
            app.Application.Id,
            "1.0.0",
            content: "<p>Intro</p>",
            sections: [new ReleaseNoteSectionDto(ReleaseNoteCategory.Fixed, "<ul><li>Crash</li></ul><script>x()</script>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Changed, "<p><br></p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Breaking, "<p>Old API removed</p>")]);

        note.SummaryHtml.Should().Be("<p>Intro</p>");
        note.Sections.Should().Equal(
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Breaking, "<p>Old API removed</p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Fixed, "<ul><li>Crash</li></ul>"));
    }

    [Fact]
    public async Task Update_replaces_sections_and_the_list_shows_categories()
    {
        var app = await client.CreateApplicationAsync();
        var note = await client.CreateReleaseNoteAsync(
            app.Application.Id,
            "1.0.0",
            sections: [new(ReleaseNoteCategory.Added, "<p>A</p>"), new(ReleaseNoteCategory.Fixed, "<p>F</p>")]);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/applications/{app.Application.Id}/release-notes/{note.Id}",
            new SaveReleaseNoteRequest("1.0.0", "Release", null, [new(ReleaseNoteCategory.Security, "<p>S</p>"), new(ReleaseNoteCategory.Fixed, "<p>F2</p>")], true),
            TestJson.Options);
        response.EnsureSuccessStatusCode();

        var saved = await client.GetFromJsonAsync<ReleaseNoteDto>($"/api/v1/applications/{app.Application.Id}/release-notes/{note.Id}", TestJson.Options);
        var list = await client.GetFromJsonAsync<List<ReleaseNoteSummaryDto>>($"/api/v1/applications/{app.Application.Id}/release-notes", TestJson.Options);

        saved!.Sections.Should().Equal(
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Fixed, "<p>F2</p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Security, "<p>S</p>"));
        list!.Single().Categories.Should().Equal(ReleaseNoteCategory.Fixed, ReleaseNoteCategory.Security);
    }

    [Fact]
    public async Task Categories_are_exchanged_as_camel_case_names()
    {
        var app = await client.CreateApplicationAsync();
        var url = $"/api/v1/applications/{app.Application.Id}/release-notes";
        await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", sections: new ReleaseNoteSectionDto(ReleaseNoteCategory.Breaking, "<p>B</p>"));

        var json = await client.GetStringAsync(url);
        var numeric = await client.PostAsync(url, Json("""{"version":"2.0.0","title":"T","sections":[{"category":1,"contentHtml":"<p>x</p>"}],"isPublished":false}"""));
        var unknown = await client.PostAsync(url, Json("""{"version":"2.0.0","title":"T","sections":[{"category":"bugs","contentHtml":"<p>x</p>"}],"isPublished":false}"""));

        json.Should().Contain("\"categories\":[\"breaking\"]");
        numeric.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        unknown.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task A_category_can_only_have_one_section()
    {
        var app = await client.CreateApplicationAsync();

        var response = await client.PostAsJsonAsync(
            $"/api/v1/applications/{app.Application.Id}/release-notes",
            new SaveReleaseNoteRequest("1.0.0", "Dup", "", [new(ReleaseNoteCategory.Fixed, "<p>a</p>"), new(ReleaseNoteCategory.Fixed, "<p>b</p>")], false),
            TestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).Should().Contain("one section per category");
    }

    private static StringContent Json(string json) => new(json, System.Text.Encoding.UTF8, "application/json");

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
