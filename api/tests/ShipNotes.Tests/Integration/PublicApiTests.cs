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

    public static TheoryData<string> PublicPaths =>
    [
        "",
        "/release-notes",
        "/release-notes/latest",
        "/release-notes/1.0.0",
        "/release-notes/between?from=1.0.0&to=2.0.0",
    ];

    [Theory]
    [MemberData(nameof(PublicPaths))]
    public async Task Every_public_endpoint_requires_a_valid_key(string path)
    {
        var app = await SeedAsync();
        var url = $"/api/v1/public/applications/{app.Id}{path}";

        (await client.GetAsync(url)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await GetAsync(url, "sn_wrong")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await GetAsync(url, app.ApiKey)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(PublicPaths))]
    public async Task Every_public_endpoint_rejects_a_key_for_another_application(string path)
    {
        var app = await SeedAsync();
        var other = await client.CreateApplicationAsync("Other");

        var wrongApp = await GetAsync($"/api/v1/public/applications/{app.Id}{path}", other.ApiKey);
        var unknownApp = await GetAsync($"/api/v1/public/applications/{Guid.NewGuid()}{path}", app.ApiKey);

        wrongApp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        unknownApp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Only_published_stable_notes_of_the_application_are_returned_by_default()
    {
        var app = await SeedAsync();
        var other = await client.CreateApplicationAsync("Other");
        await client.CreateReleaseNoteAsync(other.Application.Id, "9.0.0");

        var notes = await GetNotesAsync(app, "/release-notes");

        notes.Select(n => n.Version).Should().Equal("2.0.0", "1.1.0", "1.0.0");
    }

    [Fact]
    public async Task Pre_releases_can_be_included()
    {
        var app = await SeedAsync();

        var notes = await GetNotesAsync(app, "/release-notes?includePreReleases=true");

        notes.Select(n => n.Version).Should().Equal("2.1.0-beta.1", "2.0.0", "1.1.0", "1.0.0");
    }

    [Fact]
    public async Task Notes_can_be_filtered_to_a_version_range_and_limited()
    {
        var app = await SeedAsync();

        var range = await GetNotesAsync(app, "/release-notes?after=1.0.0&upTo=2.0.0");
        var limited = await GetNotesAsync(app, "/release-notes?limit=1");

        range.Select(n => n.Version).Should().Equal("2.0.0", "1.1.0");
        limited.Select(n => n.Version).Should().Equal("2.0.0");
    }

    [Fact]
    public async Task Between_excludes_from_and_includes_to()
    {
        var app = await SeedAsync();

        var notes = await GetNotesAsync(app, "/release-notes/between?from=1.0.0&to=2.0.0");

        notes.Select(n => n.Version).Should().Equal("2.0.0", "1.1.0");
    }

    [Fact]
    public async Task Between_can_include_from_and_pre_releases()
    {
        var app = await SeedAsync();

        var inclusive = await GetNotesAsync(app, "/release-notes/between?from=1.0.0&to=1.1.0&includeFrom=true");
        var withPreReleases = await GetNotesAsync(app, "/release-notes/between?from=2.0.0&to=3.0.0&includePreReleases=true");

        inclusive.Select(n => n.Version).Should().Equal("1.1.0", "1.0.0");
        withPreReleases.Select(n => n.Version).Should().Equal("2.1.0-beta.1");
    }

    [Fact]
    public async Task Between_orders_by_semver_precedence_not_text()
    {
        var app = await client.CreateApplicationAsync("Semver");
        foreach (var version in new[] { "1.2.0", "1.9.0", "1.10.0", "1.11.0", "2.0.0-rc.1", "2.0.0" })
        {
            await client.CreateReleaseNoteAsync(app.Application.Id, version);
        }

        var seeded = new SeededApp(app.Application.Id, app.ApiKey);
        var notes = await GetNotesAsync(seeded, "/release-notes/between?from=1.9.0&to=2.0.0&includePreReleases=true");

        notes.Select(n => n.Version).Should().Equal("2.0.0", "2.0.0-rc.1", "1.11.0", "1.10.0");
    }

    [Theory]
    [InlineData("/release-notes/between?to=2.0.0")]
    [InlineData("/release-notes/between?from=1.0.0")]
    [InlineData("/release-notes/between?from=one&to=2.0.0")]
    [InlineData("/release-notes/between?from=2.0.0&to=1.10.0")]
    [InlineData("/release-notes?after=latest")]
    [InlineData("/release-notes?limit=500")]
    public async Task Invalid_query_values_are_rejected(string path)
    {
        var app = await SeedAsync();

        (await GetAsync($"/api/v1/public/applications/{app.Id}{path}", app.ApiKey)).StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Latest_and_by_version_endpoints_return_single_notes()
    {
        var app = await SeedAsync();
        var baseUrl = $"/api/v1/public/applications/{app.Id}/release-notes";

        var latest = await (await GetAsync($"{baseUrl}/latest", app.ApiKey)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>();
        var byVersion = await (await GetAsync($"{baseUrl}/1.1.0", app.ApiKey)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>();
        var draft = await GetAsync($"{baseUrl}/3.0.0", app.ApiKey);

        latest!.Version.Should().Be("2.0.0");
        byVersion!.Title.Should().Be("Release 1.1.0");
        draft.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Application_details_are_returned()
    {
        var app = await SeedAsync();

        var details = await (await GetAsync($"/api/v1/public/applications/{app.Id}", app.ApiKey)).Content.ReadFromJsonAsync<PublicApplicationDto>();

        details!.Name.Should().Be("Portal");
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private sealed record SeededApp(Guid Id, string ApiKey);

    private async Task<SeededApp> SeedAsync()
    {
        var app = await client.CreateApplicationAsync("Portal");
        var id = app.Application.Id;
        await client.CreateReleaseNoteAsync(id, "1.0.0");
        await client.CreateReleaseNoteAsync(id, "1.1.0");
        await client.CreateReleaseNoteAsync(id, "2.0.0");
        await client.CreateReleaseNoteAsync(id, "2.1.0-beta.1");
        await client.CreateReleaseNoteAsync(id, "3.0.0", isPublished: false);
        return new SeededApp(id, app.ApiKey);
    }

    private Task<HttpResponseMessage> GetAsync(string url, string apiKey) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url).WithApiKey(apiKey));

    private async Task<List<PublicReleaseNoteDto>> GetNotesAsync(SeededApp app, string path)
    {
        var response = await GetAsync($"/api/v1/public/applications/{app.Id}{path}", app.ApiKey);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<List<PublicReleaseNoteDto>>())!;
    }
}
