using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Applications;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Application.Media;
using ShipNotes.Application.ReleaseNotes;
using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Showcase;

/// <summary>
/// Seeds the demo application through the same services the UI uses, so its notes go through
/// media upload, sanitisation and media tracking exactly like notes written by hand.
/// </summary>
internal sealed partial class ShowcaseService(
    IOptions<ShowcaseOptions> options,
    IClientApplicationRepository applications,
    IApplicationService applicationService,
    IReleaseNoteService releaseNoteService,
    IMediaService mediaService,
    IApiKeyService apiKeys,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IShowcaseService
{
    private static readonly Guid DemoId = ShowcaseDefaults.ApplicationId;

    public async Task<ShowcaseSessionDto> StartAsync(CancellationToken cancellationToken)
    {
        EnsureEnabled();

        var existing = await applications.FindAsync(DemoId, cancellationToken);
        if (existing is null)
        {
            return await SeedAsync(cancellationToken);
        }

        var key = apiKeys.Generate();
        existing.SetApiKey(key.Credential, clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ShowcaseSessionDto(existing.ToDto(), key.Value);
    }

    public async Task<ShowcaseSessionDto> ResetAsync(CancellationToken cancellationToken)
    {
        EnsureEnabled();

        if (await applications.FindAsync(DemoId, cancellationToken) is not null)
        {
            await applicationService.DeleteAsync(DemoId, cancellationToken);
        }

        return await SeedAsync(cancellationToken);
    }

    private void EnsureEnabled()
    {
        if (!options.Value.Enabled)
        {
            throw new NotFoundException("The showcase is turned off. Set Showcase:Enabled to true to use it.");
        }
    }

    private async Task<ShowcaseSessionDto> SeedAsync(CancellationToken cancellationToken)
    {
        if (await applications.NameExistsAsync(ShowcaseDefaults.ApplicationName, null, cancellationToken))
        {
            throw new ConflictException(
                $"An application named '{ShowcaseDefaults.ApplicationName}' already exists. Rename it to use the showcase.");
        }

        var key = apiKeys.Generate();
        applications.Add(ClientApplication.Create(DemoId, ShowcaseDefaults.ApplicationName, null, key.Credential, clock.GetUtcNow()));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var icon = await UploadAsync(ShowcaseCatalog.IconFile, cancellationToken);
        var application = await applicationService.UpdateAsync(
            DemoId,
            new SaveApplicationRequest(ShowcaseDefaults.ApplicationName, icon.Url),
            cancellationToken);

        foreach (var note in ShowcaseCatalog.Notes)
        {
            var content = ShowcaseContent.ReadNote(note.Version);
            var summary = await ResolveMediaAsync(content.SummaryHtml, cancellationToken);
            var sections = new List<ReleaseNoteSectionDto>();
            foreach (var section in content.Sections)
            {
                sections.Add(new ReleaseNoteSectionDto(section.Category, await ResolveMediaAsync(section.ContentHtml, cancellationToken)));
            }

            await releaseNoteService.CreateAsync(
                DemoId,
                new SaveReleaseNoteRequest(note.Version, note.Title, summary, sections, note.IsPublished),
                cancellationToken);
        }

        return new ShowcaseSessionDto(application, key.Value);
    }

    /// <summary>Uploads each image a note refers to as <c>{{media:file.png}}</c> and swaps in its URL.</summary>
    private async Task<string> ResolveMediaAsync(string html, CancellationToken cancellationToken)
    {
        var urls = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var fileName in MediaPlaceholder().Matches(html).Select(m => m.Groups["file"].Value).Distinct())
        {
            urls[fileName] = (await UploadAsync(fileName, cancellationToken)).Url;
        }

        return MediaPlaceholder().Replace(html, m => urls[m.Groups["file"].Value]);
    }

    private async Task<MediaDto> UploadAsync(string fileName, CancellationToken cancellationToken)
    {
        await using var stream = ShowcaseContent.OpenMedia(fileName);
        return await mediaService.UploadAsync(DemoId, new UploadMediaRequest(fileName, stream.Length, stream), cancellationToken);
    }

    [GeneratedRegex(@"\{\{media:(?<file>[A-Za-z0-9.\-]+)\}\}", RegexOptions.CultureInvariant)]
    private static partial Regex MediaPlaceholder();
}
