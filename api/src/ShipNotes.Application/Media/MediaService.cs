using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Domain.Media;

namespace ShipNotes.Application.Media;

internal sealed class MediaService(
    IClientApplicationRepository applications,
    IMediaRepository mediaRepository,
    IMediaStorage storage,
    IMediaUrlProvider urls,
    IUnitOfWork unitOfWork,
    IOptions<MediaOptions> options,
    TimeProvider clock) : IMediaService
{
    public async Task<MediaDto> UploadAsync(Guid applicationId, UploadMediaRequest request, CancellationToken cancellationToken)
    {
        if (await applications.FindAsync(applicationId, cancellationToken) is null)
        {
            throw new NotFoundException("Application", applicationId);
        }

        EnsureSizeIsAllowed(request.Length);

        await using var buffer = new MemoryStream();
        await request.Content.CopyToAsync(buffer, cancellationToken);
        EnsureSizeIsAllowed(buffer.Length);

        var format = ImageFormat.Detect(buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, ImageFormat.HeaderLength)))
            ?? throw Invalid($"Only {ImageFormat.SupportedTypesDescription} images can be uploaded.");

        var id = Guid.CreateVersion7();
        var fileName = MediaFileName.Normalize(request.FileName, format.Extension);
        var storagePath = $"{applicationId}/{id}{format.Extension}";

        buffer.Position = 0;
        await storage.SaveAsync(storagePath, buffer, cancellationToken);

        var media = MediaData.Create(id, applicationId, fileName, format.ContentType, buffer.Length, storagePath, clock.GetUtcNow());
        mediaRepository.Add(media);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            storage.Delete(storagePath);
            throw;
        }

        return new MediaDto(media.Id, urls.GetUrl(media), media.FileName, media.ContentType, media.SizeBytes);
    }

    public async Task<MediaContent> OpenAsync(Guid id, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindAsync(id, cancellationToken);
        var content = media is null ? null : storage.OpenRead(media.StoragePath);

        return content is null
            ? throw new NotFoundException("Media", id)
            : new MediaContent(content, media!.ContentType, media.FileName);
    }

    private void EnsureSizeIsAllowed(long length)
    {
        if (length <= 0)
        {
            throw Invalid("The file is empty.");
        }

        var max = options.Value.MaxFileSizeBytes;
        if (length > max)
        {
            throw Invalid($"Files must be {max / (1024 * 1024)} MB or smaller.");
        }
    }

    private static ValidationException Invalid(string message) =>
        new([new ValidationFailure("file", message)]);
}
