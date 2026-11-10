using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.PublicApi;

namespace ShipNotes.Tests.Integration;

public sealed class PublicApiTests : IDisposable
{
    private readonly ShipNotesApiFactory factory = new();
    private readonly HttpClient client;

    public PublicApiTests() => client = factory.CreateClient();

    [Fact]
    public async Task Requests_without_a_valid_key_are_rejected()
    {
        (await client.GetAsync("/api/v1/public/release-notes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await GetAsync("/api/v1/public/release-notes", "sn_wrong")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Only_published_stable_notes_of_the_calling_application_are_returned_by_default()
    {
        var app = await SeedAsync();
        var other = await client.CreateApplicationAsync("Other");
        await client.CreateReleaseNoteAsync(other.Application.Id, "9.0.0");

        var notes = await GetNotesAsync("/api/v1/public/release-notes", app);

        notes.Select(n => n.Version).Should().Equal("2.0.0", "1.1.0", "1.0.0");
    }

    [Fact]
    public async Task Pre_releases_can_be_included()
    {
        var app = await SeedAsync();

        var notes = await GetNotesAsync("/api/v1/public/release-notes?includePreReleases=true", app);

        notes.Select(n => n.Version).Should().Equal("2.1.0-beta.1", "2.0.0", "1.1.0", "1.0.0");
    }

    [Fact]
    public async Task Notes_can_be_filtered_to_a_version_range_and_limited()
    {
        var app = await SeedAsync();

        var range = await GetNotesAsync("/api/v1/public/release-notes?after=1.0.0&upTo=2.0.0", app);
        var limited = await GetNotesAsync("/api/v1/public/release-notes?limit=1", app);

        range.Select(n => n.Version).Should().Equal("2.0.0", "1.1.0");
        limited.Select(n => n.Version).Should().Equal("2.0.0");
    }

    [Fact]
    public async Task Invalid_query_values_are_rejected()
    {
        var app = await SeedAsync();

        (await GetAsync("/api/v1/public/release-notes?after=latest", app)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await GetAsync("/api/v1/public/release-notes?limit=500", app)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Latest_and_by_version_endpoints_return_single_notes()
    {
        var app = await SeedAsync();

        var latest = await (await GetAsync("/api/v1/public/release-notes/latest", app)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>();
        var byVersion = await (await GetAsync("/api/v1/public/release-notes/1.1.0", app)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>();
        var draft = await GetAsync("/api/v1/public/release-notes/3.0.0", app);

        latest!.Version.Should().Be("2.0.0");
        byVersion!.Title.Should().Be("Release 1.1.0");
        draft.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Application_details_are_returned_for_the_key()
    {
        var app = await SeedAsync();

        var details = await (await GetAsync("/api/v1/public/application", app)).Content.ReadFromJsonAsync<PublicApplicationDto>();

        details!.Name.Should().Be("Portal");
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private async Task<string> SeedAsync()
    {
        var app = await client.CreateApplicationAsync("Portal");
        var id = app.Application.Id;
        await client.CreateReleaseNoteAsync(id, "1.0.0");
        await client.CreateReleaseNoteAsync(id, "1.1.0");
        await client.CreateReleaseNoteAsync(id, "2.0.0");
        await client.CreateReleaseNoteAsync(id, "2.1.0-beta.1");
        await client.CreateReleaseNoteAsync(id, "3.0.0", isPublished: false);
        return app.ApiKey;
    }

    private Task<HttpResponseMessage> GetAsync(string url, string apiKey) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url).WithApiKey(apiKey));

    private async Task<List<PublicReleaseNoteDto>> GetNotesAsync(string url, string apiKey)
    {
        var response = await GetAsync(url, apiKey);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<List<PublicReleaseNoteDto>>())!;
    }
}
