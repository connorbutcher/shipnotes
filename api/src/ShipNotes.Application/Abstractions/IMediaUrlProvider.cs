using ShipNotes.Domain.Media;

namespace ShipNotes.Application.Abstractions;

/// <summary>Builds the absolute URL that editors and consuming apps load a media file from.</summary>
public interface IMediaUrlProvider
{
    string GetUrl(MediaData media);
}
