using FluentValidation;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Application.Media;
using ShipNotes.Domain.ReleaseNotes;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Application.ReleaseNotes;

internal sealed class ReleaseNoteService(
    IClientApplicationRepository applications,
    IReleaseNoteRepository releaseNotes,
    IReleaseNoteMediaService noteMedia,
    IUnitOfWork unitOfWork,
    IValidator<SaveReleaseNoteRequest> validator,
    TimeProvider clock) : IReleaseNoteService
{
    public async Task<IReadOnlyList<ReleaseNoteSummaryDto>> ListAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        await EnsureApplicationExistsAsync(applicationId, cancellationToken);

        var query = new ReleaseNoteQuery(applicationId, PublishedOnly: false, IncludePreReleases: true);
        var notes = await releaseNotes.ListAsync(query, cancellationToken);
        return [.. notes.Select(n => n.ToSummaryDto())];
    }

    public async Task<ReleaseNoteDto> GetAsync(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        (await GetEntityAsync(applicationId, id, cancellationToken)).ToDto();

    public async Task<ReleaseNoteDto> CreateAsync(Guid applicationId, SaveReleaseNoteRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureApplicationExistsAsync(applicationId, cancellationToken);

        var version = SemanticVersion.Parse(request.Version);
        await EnsureVersionIsUniqueAsync(applicationId, version, null, cancellationToken);

        var now = clock.GetUtcNow();
        var content = await noteMedia.PrepareContentAsync(applicationId, releaseNoteId: null, request.ToContent(), cancellationToken);
        var note = ReleaseNote.Create(applicationId, version, request.Title, content.Content, now);
        ApplyPublishState(note, request.IsPublished, now);

        releaseNotes.Add(note);
        await noteMedia.SyncAsync(note, content, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return note.ToDto();
    }

    public async Task<ReleaseNoteDto> UpdateAsync(Guid applicationId, Guid id, SaveReleaseNoteRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var note = await GetEntityAsync(applicationId, id, cancellationToken);

        var version = SemanticVersion.Parse(request.Version);
        await EnsureVersionIsUniqueAsync(applicationId, version, id, cancellationToken);

        var now = clock.GetUtcNow();
        var content = await noteMedia.PrepareContentAsync(applicationId, note.Id, request.ToContent(), cancellationToken);
        note.Update(version, request.Title, content.Content, now);
        ApplyPublishState(note, request.IsPublished, now);

        var removedMedia = await noteMedia.SyncAsync(note, content, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        noteMedia.DeleteFiles(removedMedia);

        return note.ToDto();
    }

    public async Task DeleteAsync(Guid applicationId, Guid id, CancellationToken cancellationToken)
    {
        var note = await GetEntityAsync(applicationId, id, cancellationToken);
        var removedMedia = await noteMedia.DetachAllAsync(note.Id, cancellationToken);
        releaseNotes.Remove(note);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        noteMedia.DeleteFiles(removedMedia);
    }

    private static void ApplyPublishState(ReleaseNote note, bool isPublished, DateTimeOffset now)
    {
        if (isPublished)
        {
            note.Publish(now);
        }
        else
        {
            note.Unpublish(now);
        }
    }

    private async Task<ReleaseNote> GetEntityAsync(Guid applicationId, Guid id, CancellationToken cancellationToken) =>
        await releaseNotes.FindAsync(applicationId, id, cancellationToken)
        ?? throw new NotFoundException("Release note", id);

    private async Task EnsureApplicationExistsAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        if (await applications.FindAsync(applicationId, cancellationToken) is null)
        {
            throw new NotFoundException("Application", applicationId);
        }
    }

    private async Task EnsureVersionIsUniqueAsync(
        Guid applicationId,
        SemanticVersion version,
        Guid? excludingId,
        CancellationToken cancellationToken)
    {
        if (await releaseNotes.VersionExistsAsync(applicationId, version.SortKey, excludingId, cancellationToken))
        {
            throw new ConflictException($"Release notes for version {version} already exist for this application.");
        }
    }
}
