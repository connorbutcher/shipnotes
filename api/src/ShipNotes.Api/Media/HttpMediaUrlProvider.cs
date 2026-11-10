using Microsoft.Extensions.Options;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Media;
using ShipNotes.Domain.Media;

namespace ShipNotes.Api.Media;

/// <summary>
/// Builds media URLs from Media:PublicBaseUrl, falling back to the current request's scheme and host
/// so links work out of the box in development.
/// </summary>
internal sealed class HttpMediaUrlProvider(IOptions<MediaOptions> options, IHttpContextAccessor httpContextAccessor)
    : IMediaUrlProvider
{
    public string GetUrl(MediaData media) => GetBaseUrl() + MediaRoutes.GetPath(media.Id, media.FileName);

    private string GetBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(options.Value.PublicBaseUrl))
        {
            return options.Value.PublicBaseUrl.TrimEnd('/');
        }

        var request = httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException("Media:PublicBaseUrl must be configured to build media URLs outside a request.");

        return $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
    }
}
