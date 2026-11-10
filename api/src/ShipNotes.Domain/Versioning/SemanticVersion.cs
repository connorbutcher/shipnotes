using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShipNotes.Domain.Versioning;

/// <summary>
/// A Semantic Versioning 2.0.0 version (https://semver.org). Build metadata is kept
/// for display but ignored for precedence, as the specification requires.
/// </summary>
public sealed partial record SemanticVersion : IComparable<SemanticVersion>
{
    public const int MaxLength = 128;

    private const int NumericSortWidth = 10;

    private SemanticVersion(int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
        BuildMetadata = buildMetadata;
    }

    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public string PreRelease { get; }

    public string BuildMetadata { get; }

    public bool IsPreRelease => PreRelease.Length > 0;

    /// <summary>
    /// An ordinal-sortable string whose ordering matches semver precedence, so the
    /// database can sort and filter versions with a single indexed column.
    /// </summary>
    public string SortKey
    {
        get
        {
            var core = string.Create(
                CultureInfo.InvariantCulture,
                $"{Major:D10}.{Minor:D10}.{Patch:D10}");

            // A release ranks above any of its pre-releases, so it gets the higher marker.
            return IsPreRelease ? $"{core}.0{PreReleaseSortKey()}" : $"{core}.1";
        }
    }

    public static SemanticVersion Parse(string value) =>
        TryParse(value, out var version)
            ? version
            : throw new FormatException($"'{value}' is not a valid semantic version (expected e.g. 1.4.0 or 2.0.0-beta.1).");

    public static bool TryParse([NotNullWhen(true)] string? value, [NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength)
        {
            return false;
        }

        var match = SemVerPattern().Match(value.Trim());
        if (!match.Success
            || !TryParseNumber(match.Groups["major"].Value, out var major)
            || !TryParseNumber(match.Groups["minor"].Value, out var minor)
            || !TryParseNumber(match.Groups["patch"].Value, out var patch))
        {
            return false;
        }

        version = new SemanticVersion(
            major,
            minor,
            patch,
            match.Groups["pre"].Value,
            match.Groups["build"].Value);
        return true;
    }

    public int CompareTo(SemanticVersion? other) =>
        other is null ? 1 : string.CompareOrdinal(SortKey, other.SortKey);

    public override string ToString()
    {
        var text = string.Create(CultureInfo.InvariantCulture, $"{Major}.{Minor}.{Patch}");
        if (IsPreRelease)
        {
            text += $"-{PreRelease}";
        }

        if (BuildMetadata.Length > 0)
        {
            text += $"+{BuildMetadata}";
        }

        return text;
    }

    public static bool operator <(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) < 0;

    public static bool operator >(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) > 0;

    public static bool operator <=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) >= 0;

    private static bool TryParseNumber(string text, out int number) =>
        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out number);

    // Numeric identifiers sort below alphanumeric ones ('~' prefix ranks after digits),
    // numeric identifiers are zero-padded so they compare numerically, and '!' separates
    // identifiers because it ranks below every character allowed inside one.
    private string PreReleaseSortKey() =>
        string.Join(
            '!',
            PreRelease.Split('.').Select(identifier =>
                identifier.All(char.IsAsciiDigit)
                    ? identifier.PadLeft(NumericSortWidth, '0')
                    : $"~{identifier}"));

    [GeneratedRegex(
        @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)" +
        @"(?:-(?<pre>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?" +
        @"(?:\+(?<build>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
    private static partial Regex SemVerPattern();
}
