namespace ShipNotes.Application.Media;

public sealed record MediaDto(Guid Id, string Url, string FileName, string ContentType, long SizeBytes);

public sealed record UploadMediaRequest(string FileName, long Length, Stream Content);

public sealed record MediaContent(Stream Content, string ContentType, string FileName);
