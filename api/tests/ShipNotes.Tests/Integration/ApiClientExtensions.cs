using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.Applications;
using ShipNotes.Application.ReleaseNotes;

namespace ShipNotes.Tests.Integration;

internal static class ApiClientExtensions
{
    public static async Task<ApplicationWithApiKeyDto> CreateApplicationAsync(this HttpClient client, string name = "Portal")
    {
        var response = await client.PostAsJsonAsync("/api/v1/applications", new SaveApplicationRequest(name, "https://example.com/icon.png"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApplicationWithApiKeyDto>())!;
    }

    public static async Task<ReleaseNoteDto> CreateReleaseNoteAsync(
        this HttpClient client,
        Guid applicationId,
        string version,
        bool isPublished = true,
        string content = "<p>Changes</p>")
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v1/applications/{applicationId}/release-notes",
            new SaveReleaseNoteRequest(version, $"Release {version}", content, isPublished));
        response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<ReleaseNoteDto>())!;
    }

    public static HttpRequestMessage WithApiKey(this HttpRequestMessage request, string apiKey)
    {
        request.Headers.Add("X-Api-Key", apiKey);
        return request;
    }
}
