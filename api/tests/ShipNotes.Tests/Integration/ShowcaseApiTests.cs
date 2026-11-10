using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Domain.ReleaseNotes;
using ShipNotes.Application.Applications;
using ShipNotes.Application.PublicApi;
using ShipNotes.Application.Showcase;

namespace ShipNotes.Tests.Integration;

public sealed class ShowcaseApiTests : IDisposable
{
    private readonly ShipNotesApiFactory factory = new() { ShowcaseEnabled = true };
    private readonly HttpClient client;

    public ShowcaseApiTests() => client = factory.CreateClient();

    [Fact]
    public async Task Starting_seeds_the_demo_application_with_an_icon_and_published_notes()
    {
        var session = await StartAsync();

        session.Application.Id.Should().Be(ShowcaseDefaults.ApplicationId);
        session.Application.Name.Should().Be(ShowcaseDefaults.ApplicationName);
        session.Application.IconUrl.Should().Contain("/api/v1/media/");
        (await client.GetAsync(session.Application.IconUrl)).StatusCode.Should().Be(HttpStatusCode.OK);

        var notes = await GetPublicAsync<List<PublicReleaseNoteDto>>(session, "/release-notes?includePreReleases=true");
        notes.Select(n => n.Version).Should().Equal(
            "2.0.0", "2.0.0-rc.1", "1.3.0", "1.3.0-beta.1", "1.2.0", "1.1.1", "1.1.0", "1.0.0");
    }

    [Fact]
    public async Task Seeded_notes_contain_images_that_load()
    {
        var session = await StartAsync();

        var note = await GetPublicAsync<PublicReleaseNoteDto>(session, "/release-notes/1.1.0");
        var source = ImageSources(note.ContentHtml).Should().ContainSingle().Subject;

        source.Should().Contain("/api/v1/media/");
        var image = await client.GetAsync(source);
        image.StatusCode.Should().Be(HttpStatusCode.OK);
        image.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
    }

    [Fact]
    public async Task Seeded_notes_cover_every_category()
    {
        var session = await StartAsync();

        var notes = await GetPublicAsync<List<PublicReleaseNoteDto>>(session, "/release-notes?includePreReleases=true");

        notes.SelectMany(n => n.Sections).Select(s => s.Category).Distinct()
            .Should().BeEquivalentTo(ReleaseNoteCategories.All);
    }

    [Fact]
    public async Task The_draft_note_is_never_returned_publicly()
    {
        var session = await StartAsync();

        var latest = await GetPublicAsync<PublicReleaseNoteDto>(session, "/release-notes/latest");
        var draft = await SendPublicAsync(session, "/release-notes/2.1.0");

        latest.Version.Should().Be("2.0.0");
        draft.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Starting_again_keeps_the_demo_and_rotates_its_api_key()
    {
        var first = await StartAsync();
        var second = await StartAsync();

        second.Application.Id.Should().Be(first.Application.Id);
        second.ApiKey.Should().NotBe(first.ApiKey);
        (await SendPublicAsync(first, "/release-notes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await SendPublicAsync(second, "/release-notes")).StatusCode.Should().Be(HttpStatusCode.OK);

        var applications = await client.GetFromJsonAsync<List<ApplicationDto>>("/api/v1/applications", TestJson.Options);
        applications!.Should().ContainSingle(a => a.Id == ShowcaseDefaults.ApplicationId);
    }

    [Fact]
    public async Task Reset_discards_changes_to_the_demo()
    {
        var session = await StartAsync();
        await client.CreateReleaseNoteAsync(session.Application.Id, "9.9.9");

        var response = await client.PostAsync("/api/v1/showcase/reset", null);
        response.EnsureSuccessStatusCode();
        var reset = (await response.Content.ReadFromJsonAsync<ShowcaseSessionDto>(TestJson.Options))!;

        (await SendPublicAsync(reset, "/release-notes/9.9.9")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await GetPublicAsync<PublicReleaseNoteDto>(reset, "/release-notes/latest")).Version.Should().Be("2.0.0");
    }

    [Fact]
    public async Task Seeding_fails_with_conflict_when_another_application_has_the_demo_name()
    {
        await client.CreateApplicationAsync(ShowcaseDefaults.ApplicationName);

        var response = await client.PostAsync("/api/v1/showcase/session", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Showcase_is_not_found_when_disabled()
    {
        using var disabled = new ShipNotesApiFactory();
        using var disabledClient = disabled.CreateClient();

        var response = await disabledClient.PostAsync("/api/v1/showcase/session", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private static IEnumerable<string> ImageSources(string html) =>
        System.Text.RegularExpressions.Regex.Matches(html, "<img[^>]*src=\"([^\"]+)\"").Select(m => m.Groups[1].Value);

    private async Task<ShowcaseSessionDto> StartAsync()
    {
        var response = await client.PostAsync("/api/v1/showcase/session", null);
        response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<ShowcaseSessionDto>(TestJson.Options))!;
    }

    private Task<HttpResponseMessage> SendPublicAsync(ShowcaseSessionDto session, string path) =>
        client.SendAsync(new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/public/applications/{session.Application.Id}{path}").WithApiKey(session.ApiKey));

    private async Task<T> GetPublicAsync<T>(ShowcaseSessionDto session, string path)
    {
        var response = await SendPublicAsync(session, path);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<T>(TestJson.Options))!;
    }
}
