using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.ReleaseNotes;

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

        var notes = await client.GetFromJsonAsync<List<ReleaseNoteSummaryDto>>($"/api/v1/applications/{app.Application.Id}/release-notes");

        notes!.Select(n => n.Version).Should().Equal("1.10.0", "1.10.0-beta.1", "1.9.3", "1.2.0");
    }

    [Fact]
    public async Task Create_sanitises_html_content()
    {
        var app = await client.CreateApplicationAsync();

        var note = await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: "<p>Hi</p><script>alert(1)</script>");

        note.ContentHtml.Should().Be("<p>Hi</p>");
    }

    [Fact]
    public async Task Create_rejects_invalid_and_duplicate_versions()
    {
        var app = await client.CreateApplicationAsync();
        var url = $"/api/v1/applications/{app.Application.Id}/release-notes";
        await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0");

        var invalid = await client.PostAsJsonAsync(url, new SaveReleaseNoteRequest("1.0", "Bad", "<p></p>", false));
        var duplicate = await client.PostAsJsonAsync(url, new SaveReleaseNoteRequest("1.0.0+build.5", "Dup", "<p></p>", false));

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
            new SaveReleaseNoteRequest("1.0.1", "Patched", "<p>Fixed</p>", true));
        var updated = (await response.Content.ReadFromJsonAsync<ReleaseNoteDto>())!;

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

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
