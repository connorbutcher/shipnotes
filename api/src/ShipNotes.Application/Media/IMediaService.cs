namespace ShipNotes.Application.Media;

public interface IMediaService
{
    Task<MediaDto> UploadAsync(Guid applicationId, UploadMediaRequest request, CancellationToken cancellationToken);

    Task<MediaContent> OpenAsync(Guid id, CancellationToken cancellationToken);
}
