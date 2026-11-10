using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShipNotes.Application.Abstractions;
using ShipNotes.Application.Media;

namespace ShipNotes.Infrastructure.Media;

/// <summary>Stores media files in a folder on disk (Media:RootPath), e.g. F:\Dev\ShipNotesMedia.</summary>
internal sealed partial class FileSystemMediaStorage(IOptions<MediaOptions> options, ILogger<FileSystemMediaStorage> logger)
    : IMediaStorage
{
    private readonly string root = Path.GetFullPath(options.Value.RootPath);

    public async Task SaveAsync(string path, Stream content, CancellationToken cancellationToken)
    {
        var fullPath = Resolve(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await content.CopyToAsync(file, cancellationToken);
    }

    public Stream? OpenRead(string path)
    {
        var fullPath = Resolve(path);
        return File.Exists(fullPath)
            ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true)
            : null;
    }

    public void Delete(string path) => BestEffort(path, () => File.Delete(Resolve(path)));

    public void DeleteFolder(string folder) => BestEffort(folder, () =>
    {
        var fullPath = Resolve(folder);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    });

    /// <summary>Maps a relative path to a full path, refusing anything that escapes the root folder.</summary>
    private string Resolve(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(root, relativePath));
        var rootWithSeparator = Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;

        return fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
            ? fullPath
            : throw new InvalidOperationException("Media paths must stay inside the media root folder.");
    }

    private void BestEffort(string path, Action delete)
    {
        try
        {
            delete();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LogDeleteFailed(logger, ex, path);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not delete media at {Path}")]
    private static partial void LogDeleteFailed(ILogger logger, Exception exception, string path);
}
