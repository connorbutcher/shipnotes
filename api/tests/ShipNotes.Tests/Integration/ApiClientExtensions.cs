using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ShipNotes.Application.Applications;
using ShipNotes.Application.Media;
using ShipNotes.Application.ReleaseNotes;

namespace ShipNotes.Tests.Integration;

internal static class ApiClientExtensions
{
    public static async Task<ApplicationWithApiKeyDto> CreateApplicationAsync(this HttpClient client, string name = "Portal")
    {
        var response = await client.PostAsJsonAsync("/api/v1/applications", new SaveApplicationRequest(name, "https://example.com/icon.png"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApplicationWithApiKeyDto>(TestJson.Options))!;
    }

    public static async Task<ReleaseNoteDto> CreateReleaseNoteAsync(
        this HttpClient client,
        Guid applicationId,
        string version,
        bool isPublished = true,
        string content = "<p>Changes</p>",
        params ReleaseNoteSectionDto[] sections)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v1/applications/{applicationId}/release-notes",
            new SaveReleaseNoteRequest(version, $"Release {version}", content, sections, isPublished),
            TestJson.Options);
        response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<ReleaseNoteDto>(TestJson.Options))!;
    }

    /// <summary>The smallest valid PNG: a 1x1 transparent pixel.</summary>
    public static readonly byte[] TinyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");

    public static Task<HttpResponseMessage> PostMediaAsync(
        this HttpClient client,
        Guid applicationId,
        byte[] bytes,
        string fileName = "Screen Shot.png",
        string contentType = "image/png")
    {
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var form = new MultipartFormDataContent { { file, "file", fileName } };
        return client.PostAsync($"/api/v1/applications/{applicationId}/media", form);
    }

    public static async Task<MediaDto> UploadImageAsync(this HttpClient client, Guid applicationId)
    {
        var response = await client.PostMediaAsync(applicationId, TinyPng);
        response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<MediaDto>(TestJson.Options))!;
    }

    public static HttpRequestMessage WithApiKey(this HttpRequestMessage request, string apiKey)
    {
        request.Headers.Add("X-Api-Key", apiKey);
        return request;
    }
}
