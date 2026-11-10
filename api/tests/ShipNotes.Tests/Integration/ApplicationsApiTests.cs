using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.Applications;

namespace ShipNotes.Tests.Integration;

public sealed class ApplicationsApiTests : IDisposable
{
    private readonly ShipNotesApiFactory factory = new();
    private readonly HttpClient client;

    public ApplicationsApiTests() => client = factory.CreateClient();

    [Fact]
    public async Task Create_returns_the_api_key_once_and_lists_the_application()
    {
        var created = await client.CreateApplicationAsync("Portal");

        created.ApiKey.Should().StartWith(created.Application.ApiKeyPrefix);

        var list = await client.GetFromJsonAsync<List<ApplicationDto>>("/api/v1/applications");
        list.Should().ContainSingle(a => a.Name == "Portal" && a.IconUrl == "https://example.com/icon.png");
    }

    [Fact]
    public async Task Create_rejects_invalid_input_with_validation_problem()
    {
        var response = await client.PostAsJsonAsync("/api/v1/applications", new SaveApplicationRequest("", "not-a-url"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("name").And.Contain("iconUrl");
    }

    [Fact]
    public async Task Create_rejects_duplicate_names()
    {
        await client.CreateApplicationAsync("Portal");

        var response = await client.PostAsJsonAsync("/api/v1/applications", new SaveApplicationRequest("Portal", null));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Regenerating_the_api_key_invalidates_the_old_one()
    {
        var created = await client.CreateApplicationAsync();

        var response = await client.PostAsync($"/api/v1/applications/{created.Application.Id}/api-key", null);
        var regenerated = (await response.Content.ReadFromJsonAsync<ApplicationWithApiKeyDto>())!;

        regenerated.ApiKey.Should().NotBe(created.ApiKey);
        (await SendPublicAsync(created.ApiKey)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await SendPublicAsync(regenerated.ApiKey)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_removes_the_application()
    {
        var created = await client.CreateApplicationAsync();

        (await client.DeleteAsync($"/api/v1/applications/{created.Application.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await client.GetAsync($"/api/v1/applications/{created.Application.Id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private Task<HttpResponseMessage> SendPublicAsync(string apiKey) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/v1/public/application").WithApiKey(apiKey));
}
