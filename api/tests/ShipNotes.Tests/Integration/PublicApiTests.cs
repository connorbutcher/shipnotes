using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.PublicApi;
using ShipNotes.Application.ReleaseNotes;
using ShipNotes.Domain.ReleaseNotes;

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
        "/release-notes?category=fixed",
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
    [InlineData("/release-notes?category=bugs")]
    [InlineData("/release-notes?category=1")]
    [InlineData("/release-notes/latest?category=fixed,nope")]
    [InlineData("/release-notes/1.0.0?category=nope")]
    [InlineData("/release-notes/between?from=1.0.0&to=2.0.0&category=nope")]
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

        var latest = await (await GetAsync($"{baseUrl}/latest", app.ApiKey)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>(TestJson.Options);
        var byVersion = await (await GetAsync($"{baseUrl}/1.1.0", app.ApiKey)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>(TestJson.Options);
        var draft = await GetAsync($"{baseUrl}/3.0.0", app.ApiKey);

        latest!.Version.Should().Be("2.0.0");
        byVersion!.Title.Should().Be("Release 1.1.0");
        draft.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Notes_include_their_sections_and_composed_html()
    {
        var app = await SeedCategorisedAsync();

        var note = await GetNoteAsync(app, "/release-notes/2.0.0");

        note.SummaryHtml.Should().Be("<p>Big release</p>");
        note.Sections.Select(s => (s.Category, s.Label)).Should().Equal(
            (ReleaseNoteCategory.Breaking, "Breaking changes"),
            (ReleaseNoteCategory.Added, "Added"),
            (ReleaseNoteCategory.Security, "Security"));
        note.ContentHtml.Should().Be(
            "<p>Big release</p>" +
            "<section data-category=\"breaking\"><h2>Breaking changes</h2><p>v1 API removed</p></section>" +
            "<section data-category=\"added\"><h2>Added</h2><p>Calendar</p></section>" +
            "<section data-category=\"security\"><h2>Security</h2><p>Shorter sessions</p></section>");
    }

    [Fact]
    public async Task Categories_are_serialised_as_camel_case_names()
    {
        var app = await SeedCategorisedAsync();

        var json = await (await GetAsync($"/api/v1/public/applications/{app.Id}/release-notes/1.0.1", app.ApiKey)).Content.ReadAsStringAsync();

        json.Should().Contain("\"category\":\"fixed\"").And.Contain("\"label\":\"Fixed\"");
    }

    [Theory]
    [InlineData("?category=security")]
    [InlineData("?category=SECURITY")]
    public async Task List_can_be_filtered_by_category(string query)
    {
        var app = await SeedCategorisedAsync();

        var notes = await GetNotesAsync(app, "/release-notes" + query);

        notes.Select(n => n.Version).Should().Equal("2.0.0", "1.0.1");
        notes.SelectMany(n => n.Sections).Should().OnlyContain(s => s.Category == ReleaseNoteCategory.Security);
        notes[0].ContentHtml.Should().NotContain("Calendar").And.Contain("Shorter sessions");
    }

    [Theory]
    [InlineData("&category=breaking,fixed")]
    [InlineData("&category=breaking&category=fixed")]
    public async Task Several_categories_can_be_requested(string query)
    {
        var app = await SeedCategorisedAsync();

        var notes = await GetNotesAsync(app, "/release-notes/between?from=1.0.0&to=2.0.0" + query);

        notes.Select(n => (n.Version, string.Join(",", n.Sections.Select(s => s.Category)))).Should().Equal(
            ("2.0.0", "Breaking"),
            ("1.0.1", "Fixed"));
    }

    [Fact]
    public async Task Limit_applies_after_the_category_filter()
    {
        var app = await SeedCategorisedAsync();

        var notes = await GetNotesAsync(app, "/release-notes?category=fixed&limit=1");

        notes.Select(n => n.Version).Should().Equal("1.0.1");
    }

    [Fact]
    public async Task Latest_and_by_version_honour_the_category_filter()
    {
        var app = await SeedCategorisedAsync();
        const string baseUrl = "/release-notes";

        var latestFix = await GetNoteAsync(app, $"{baseUrl}/latest?category=fixed");
        var onlySecurity = await GetNoteAsync(app, $"{baseUrl}/2.0.0?category=security");
        var noRemovals = await GetAsync($"/api/v1/public/applications/{app.Id}{baseUrl}/latest?category=removed", app.ApiKey);

        latestFix.Version.Should().Be("1.0.1");
        onlySecurity.Sections.Should().ContainSingle().Which.Category.Should().Be(ReleaseNoteCategory.Security);
        onlySecurity.SummaryHtml.Should().Be("<p>Big release</p>");
        noRemovals.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Application_details_are_returned()
    {
        var app = await SeedAsync();

        var details = await (await GetAsync($"/api/v1/public/applications/{app.Id}", app.ApiKey)).Content.ReadFromJsonAsync<PublicApplicationDto>(TestJson.Options);

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

    private async Task<SeededApp> SeedCategorisedAsync()
    {
        var app = await client.CreateApplicationAsync("Categorised");
        var id = app.Application.Id;
        await client.CreateReleaseNoteAsync(id, "1.0.0", content: "<p>First</p>", sections: [new ReleaseNoteSectionDto(ReleaseNoteCategory.Added, "<p>Inbox</p>")]);
        await client.CreateReleaseNoteAsync(
            id,
            "1.0.1",
            content: "",
            sections: [new ReleaseNoteSectionDto(ReleaseNoteCategory.Fixed, "<p>Crash</p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Security, "<p>Token leak</p>")]);
        await client.CreateReleaseNoteAsync(
            id,
            "2.0.0",
            content: "<p>Big release</p>",
            sections: [new ReleaseNoteSectionDto(ReleaseNoteCategory.Security, "<p>Shorter sessions</p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Added, "<p>Calendar</p>"),
            new ReleaseNoteSectionDto(ReleaseNoteCategory.Breaking, "<p>v1 API removed</p>")]);
        return new SeededApp(id, app.ApiKey);
    }

    private async Task<PublicReleaseNoteDto> GetNoteAsync(SeededApp app, string path)
    {
        var response = await GetAsync($"/api/v1/public/applications/{app.Id}{path}", app.ApiKey);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<PublicReleaseNoteDto>(TestJson.Options))!;
    }

    private Task<HttpResponseMessage> GetAsync(string url, string apiKey) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url).WithApiKey(apiKey));

    private async Task<List<PublicReleaseNoteDto>> GetNotesAsync(SeededApp app, string path)
    {
        var response = await GetAsync($"/api/v1/public/applications/{app.Id}{path}", app.ApiKey);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<List<PublicReleaseNoteDto>>(TestJson.Options))!;
    }
}
