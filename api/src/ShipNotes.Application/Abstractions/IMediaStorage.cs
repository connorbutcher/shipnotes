namespace ShipNotes.Application.Abstractions;

/// <summary>Stores media file contents outside the database. Paths are relative to the storage root.</summary>
public interface IMediaStorage
{
    Task SaveAsync(string path, Stream content, CancellationToken cancellationToken);

    /// <summary>Opens a file for reading, or returns null when it does not exist.</summary>
    Stream? OpenRead(string path);

    /// <summary>Deletes a file if it exists. Best effort: failures are logged, not thrown.</summary>
    void Delete(string path);

    /// <summary>Deletes a folder and everything in it if it exists. Best effort: failures are logged, not thrown.</summary>
    void DeleteFolder(string folder);
}
