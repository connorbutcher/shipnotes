using System.Text;
using ShipNotes.Domain.Media;

namespace ShipNotes.Application.Media;

/// <summary>Turns an uploaded file name into a short, URL-safe display name with the detected extension.</summary>
internal static class MediaFileName
{
    private const string DefaultStem = "image";
    private const int MaxStemLength = 80;

    public static string Normalize(string? fileName, string extension)
    {
        var stem = Path.GetFileNameWithoutExtension(Path.GetFileName(fileName ?? string.Empty));
        var builder = new StringBuilder(stem.Length);

        foreach (var c in stem.Normalize(NormalizationForm.FormKC))
        {
            if (char.IsAsciiLetterOrDigit(c) || c is '-' or '_')
            {
                builder.Append(char.ToLowerInvariant(c));
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        var safeStem = builder.ToString().Trim('-');
        if (safeStem.Length > MaxStemLength)
        {
            safeStem = safeStem[..MaxStemLength].TrimEnd('-');
        }

        var result = (safeStem.Length == 0 ? DefaultStem : safeStem) + extension;
        return result.Length <= MediaConstraints.FileNameMaxLength ? result : DefaultStem + extension;
    }
}
