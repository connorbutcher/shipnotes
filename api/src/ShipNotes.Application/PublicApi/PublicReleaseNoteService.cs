using FluentValidation;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common.Exceptions;
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
        var categories = CategoryFilter.Parse(query.Category);

        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(
                applicationId,
                PublishedOnly: true,
                query.IncludePreReleases,
                AfterSortKey: ToSortKey(query.After),
                UpToSortKey: ToSortKey(query.UpTo),
                Limit: query.Limit,
                Categories: categories),
            cancellationToken);

        return [.. notes.Select(n => n.ToPublicDto(categories))];
    }

    public async Task<IReadOnlyList<PublicReleaseNoteDto>> ListBetweenAsync(
        Guid applicationId,
        PublicReleaseNotesRangeQuery query,
        CancellationToken cancellationToken)
    {
        await rangeValidator.ValidateAndThrowAsync(query, cancellationToken);
        var categories = CategoryFilter.Parse(query.Category);

        var from = ToSortKey(query.From);
        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(
                applicationId,
                PublishedOnly: true,
                query.IncludePreReleases,
                AfterSortKey: query.IncludeFrom ? null : from,
                FromSortKey: query.IncludeFrom ? from : null,
                UpToSortKey: ToSortKey(query.To),
                Categories: categories),
            cancellationToken);

        return [.. notes.Select(n => n.ToPublicDto(categories))];
    }

    public async Task<PublicReleaseNoteDto> GetLatestAsync(
        Guid applicationId,
        bool includePreReleases,
        IReadOnlyList<string>? category,
        CancellationToken cancellationToken)
    {
        var categories = CategoryFilter.Parse(category);
        var notes = await releaseNotes.ListAsync(
            new ReleaseNoteQuery(applicationId, PublishedOnly: true, includePreReleases, Limit: 1, Categories: categories),
            cancellationToken);

        return notes.Count > 0
            ? notes[0].ToPublicDto(categories)
            : throw new NotFoundException(categories is null
                ? "No published release notes exist for this application."
                : "No published release notes with those categories exist for this application.");
    }

    public async Task<PublicReleaseNoteDto> GetByVersionAsync(
        Guid applicationId,
        string version,
        IReadOnlyList<string>? category,
        CancellationToken cancellationToken)
    {
        var categories = CategoryFilter.Parse(category);

        if (!SemanticVersion.TryParse(version, out var parsed))
        {
            throw new NotFoundException("Release note", version);
        }

        var note = await releaseNotes.FindByVersionAsync(applicationId, parsed.SortKey, publishedOnly: true, cancellationToken)
            ?? throw new NotFoundException("Release note", version);

        return note.ToPublicDto(categories);
    }

    private static string? ToSortKey(string? version) =>
        version is null ? null : SemanticVersion.Parse(version).SortKey;
}
