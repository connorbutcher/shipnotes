using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipNotes.Application.Media;
using ShipNotes.Application.PublicApi;
using ShipNotes.Application.ReleaseNotes;
using ShipNotes.Domain.Media;
using ShipNotes.Infrastructure.Persistence;

namespace ShipNotes.Tests.Integration;

public sealed class MediaApiTests : IDisposable
{
    private readonly ShipNotesApiFactory factory = new();
    private readonly HttpClient client;

    public MediaApiTests() => client = factory.CreateClient();

    [Fact]
    public async Task Upload_stores_the_file_on_disk_and_returns_a_loadable_url()
    {
        var app = await client.CreateApplicationAsync();

        var response = await client.PostMediaAsync(app.Application.Id, ApiClientExtensions.TinyPng);
        var media = (await response.Content.ReadFromJsonAsync<MediaDto>(TestJson.Options))!;

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        media.Url.Should().Be($"http://localhost/api/v1/media/{media.Id}/screen-shot.png");
        media.ContentType.Should().Be("image/png");
        File.ReadAllBytes(Path.Combine(factory.MediaRoot, app.Application.Id.ToString(), $"{media.Id}.png"))
            .Should().Equal(ApiClientExtensions.TinyPng);

        var served = await client.GetAsync(media.Url);
        served.StatusCode.Should().Be(HttpStatusCode.OK);
        served.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
        served.Headers.CacheControl!.Public.Should().BeTrue();
        (await served.Content.ReadAsByteArrayAsync()).Should().Equal(ApiClientExtensions.TinyPng);
    }

    [Fact]
    public async Task Upload_rejects_files_that_are_not_images_whatever_their_name()
    {
        var app = await client.CreateApplicationAsync();

        var script = await client.PostMediaAsync(app.Application.Id, "<svg onload=alert(1)>"u8.ToArray(), "evil.png");
        var empty = await client.PostMediaAsync(app.Application.Id, [], "empty.png");
        var unknownApp = await client.PostMediaAsync(Guid.NewGuid(), ApiClientExtensions.TinyPng);

        script.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        empty.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        unknownApp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Directory.Exists(Path.Combine(factory.MediaRoot, app.Application.Id.ToString())).Should().BeFalse();
    }

    [Fact]
    public async Task Unknown_media_returns_not_found()
    {
        (await client.GetAsync($"/api/v1/media/{Guid.NewGuid()}/x.png")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Saving_a_note_keeps_only_uploaded_images_and_tracks_them_in_media_data()
    {
        var app = await client.CreateApplicationAsync();
        var media = await client.UploadImageAsync(app.Application.Id);
        var relativeUrl = new Uri(media.Url).PathAndQuery;
        var html =
            $"<p>New</p><p><img src=\"{relativeUrl}\" alt=\"Shot\"></p>" +
            "<p><img src=\"data:image/png;base64,iVBORw0KGgo=\"></p>" +
            "<p><img src=\"https://elsewhere.example.com/hotlink.png\"></p>";

        var note = await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: html);

        note.SummaryHtml.Should().Be($"<p>New</p><p><img src=\"{media.Url}\" alt=\"Shot\"></p><p></p><p></p>");
        (await FindMediaAsync(media.Id))!.ReleaseNoteId.Should().Be(note.Id);
    }

    [Fact]
    public async Task Public_api_returns_html_whose_image_urls_load()
    {
        var app = await client.CreateApplicationAsync();
        var media = await client.UploadImageAsync(app.Application.Id);
        await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: $"<p><img src=\"{media.Url}\"></p>");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/public/applications/{app.Application.Id}/release-notes/latest").WithApiKey(app.ApiKey);
        var latest = await (await client.SendAsync(request)).Content.ReadFromJsonAsync<PublicReleaseNoteDto>(TestJson.Options);

        latest!.ContentHtml.Should().Contain(media.Url);
        (await client.GetAsync(media.Url)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Media_from_another_application_or_note_is_not_allowed()
    {
        var app = await client.CreateApplicationAsync();
        var other = await client.CreateApplicationAsync("Other");
        var foreign = await client.UploadImageAsync(other.Application.Id);
        var own = await client.UploadImageAsync(app.Application.Id);
        await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: $"<p><img src=\"{own.Url}\"></p>");

        var note = await client.CreateReleaseNoteAsync(
            app.Application.Id,
            "2.0.0",
            content: $"<p><img src=\"{foreign.Url}\"><img src=\"{own.Url}\"></p>");

        note.SummaryHtml.Should().BeEmpty("a summary left with nothing to show is stored empty");
    }

    [Fact]
    public async Task Removing_an_image_from_a_note_deletes_its_file_and_record()
    {
        var app = await client.CreateApplicationAsync();
        var kept = await client.UploadImageAsync(app.Application.Id);
        var removed = await client.UploadImageAsync(app.Application.Id);
        var note = await client.CreateReleaseNoteAsync(
            app.Application.Id,
            "1.0.0",
            content: $"<p><img src=\"{kept.Url}\"><img src=\"{removed.Url}\"></p>");

        var response = await client.PutAsJsonAsync(
            $"/api/v1/applications/{app.Application.Id}/release-notes/{note.Id}",
            new SaveReleaseNoteRequest("1.0.0", "Release", $"<p><img src=\"{kept.Url}\"></p>", [], true),
            TestJson.Options);

        response.EnsureSuccessStatusCode();
        (await FindMediaAsync(kept.Id)).Should().NotBeNull();
        (await FindMediaAsync(removed.Id)).Should().BeNull();
        File.Exists(MediaFile(app.Application.Id, removed.Id)).Should().BeFalse();
        File.Exists(MediaFile(app.Application.Id, kept.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Deleting_a_note_or_application_deletes_its_media()
    {
        var app = await client.CreateApplicationAsync();
        var first = await client.UploadImageAsync(app.Application.Id);
        var second = await client.UploadImageAsync(app.Application.Id);
        var note = await client.CreateReleaseNoteAsync(app.Application.Id, "1.0.0", content: $"<p><img src=\"{first.Url}\"></p>");
        await client.CreateReleaseNoteAsync(app.Application.Id, "2.0.0", content: $"<p><img src=\"{second.Url}\"></p>");

        (await client.DeleteAsync($"/api/v1/applications/{app.Application.Id}/release-notes/{note.Id}")).EnsureSuccessStatusCode();

        (await FindMediaAsync(first.Id)).Should().BeNull();
        File.Exists(MediaFile(app.Application.Id, first.Id)).Should().BeFalse();
        File.Exists(MediaFile(app.Application.Id, second.Id)).Should().BeTrue();

        (await client.DeleteAsync($"/api/v1/applications/{app.Application.Id}")).EnsureSuccessStatusCode();

        (await FindMediaAsync(second.Id)).Should().BeNull();
        Directory.Exists(Path.Combine(factory.MediaRoot, app.Application.Id.ToString())).Should().BeFalse();
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private string MediaFile(Guid applicationId, Guid mediaId) =>
        Path.Combine(factory.MediaRoot, applicationId.ToString(), $"{mediaId}.png");

    private async Task<MediaData?> FindMediaAsync(Guid id)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShipNotesDbContext>();
        return await db.Media.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }
}
