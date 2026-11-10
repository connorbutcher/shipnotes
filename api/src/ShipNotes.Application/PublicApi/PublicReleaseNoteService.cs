using FluentValidation;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Domain.ReleaseNotes;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Application.PublicApi;

internal sealed class PublicReleaseNoteService(
    IClientApplicationRepository applications,
    IReleaseNoteRepository releaseNotes,
    IValidator<PublicReleaseNotesQuery> validator,
    IValidator<PublicReleaseNotesRangeQuery> rangeValidator) : IPublicReleaseNoteService
{
    public async Task<PublicApplicationDto> GetApplicationAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await applications.FindAsync(applicationId, cancellationToken)
            ?? throw new NotFoundException("Application", applicationId);

        return new PublicApplicationDto(application.Name, application.IconUrl);
    }

    public async Task<IReadOnlyList<PublicReleaseNoteDto>> ListAsync(
        Guid applicationId,
        PublicReleaseNotesQuery query,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(
                applicationId,
                PublishedOnly: true,
                query.IncludePreReleases,
                AfterSortKey: ToSortKey(query.After),
                UpToSortKey: ToSortKey(query.UpTo),
                Limit: query.Limit),
            cancellationToken);

        return [.. notes.Select(ToDto)];
    }

    public async Task<IReadOnlyList<PublicReleaseNoteDto>> ListBetweenAsync(
        Guid applicationId,
        PublicReleaseNotesRangeQuery query,
        CancellationToken cancellationToken)
    {
        await rangeValidator.ValidateAndThrowAsync(query, cancellationToken);

        var from = ToSortKey(query.From);
        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(
                applicationId,
                PublishedOnly: true,
                query.IncludePreReleases,
                AfterSortKey: query.IncludeFrom ? null : from,
                FromSortKey: query.IncludeFrom ? from : null,
                UpToSortKey: ToSortKey(query.To)),
            cancellationToken);

        return [.. notes.Select(ToDto)];
    }

    public async Task<PublicReleaseNoteDto> GetLatestAsync(Guid applicationId, bool includePreReleases, CancellationToken cancellationToken)
    {
        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(applicationId, PublishedOnly: true, includePreReleases, Limit: 1),
            cancellationToken);

        return notes.Count > 0
            ? ToDto(notes[0])
            : throw new NotFoundException("No published release notes exist for this application.");
    }

    public async Task<PublicReleaseNoteDto> GetByVersionAsync(Guid applicationId, string version, CancellationToken cancellationToken)
    {
        if (!SemanticVersion.TryParse(version, out var parsed))
        {
            throw new NotFoundException("Release note", version);
        }

        var note = await releaseNotes.FindByVersionAsync(applicationId, parsed.SortKey, publishedOnly: true, cancellationToken)
            ?? throw new NotFoundException("Release note", version);

        return ToDto(note);
    }

    private static string? ToSortKey(string? version) =>
        version is null ? null : SemanticVersion.Parse(version).SortKey;

    private static PublicReleaseNoteDto ToDto(ReleaseNote note) =>
        new(note.Version, note.IsPreRelease, note.Title, note.ContentHtml, note.PublishedAt!.Value);
}
