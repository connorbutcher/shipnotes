namespace ShipNotes.Application.Media;

/// <summary>Image types that can be uploaded, detected from the file's leading bytes rather than its name.</summary>
internal sealed record ImageFormat(string ContentType, string Extension)
{
    public const int HeaderLength = 12;

    public const string SupportedTypesDescription = "PNG, JPEG, GIF or WebP";

    private static readonly ImageFormat Png = new("image/png", ".png");
    private static readonly ImageFormat Jpeg = new("image/jpeg", ".jpg");
    private static readonly ImageFormat Gif = new("image/gif", ".gif");
    private static readonly ImageFormat WebP = new("image/webp", ".webp");

    public static ImageFormat? Detect(ReadOnlySpan<byte> header)
    {
        if (header.StartsWith((byte[])[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]))
        {
            return Png;
        }

        if (header.StartsWith((byte[])[0xFF, 0xD8, 0xFF]))
        {
            return Jpeg;
        }

        if (header.StartsWith("GIF87a"u8) || header.StartsWith("GIF89a"u8))
        {
            return Gif;
        }

        if (header.Length >= HeaderLength && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8))
        {
            return WebP;
        }

        return null;
    }
}
