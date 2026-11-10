using FluentValidation;
using FluentValidation.Results;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.PublicApi;

/// <summary>
/// The <c>category</c> query option: one or more category names, repeated
/// (<c>?category=fixed&amp;category=security</c>) or comma-separated (<c>?category=fixed,security</c>).
/// </summary>
internal static class CategoryFilter
{
    public const string ParameterName = "category";

    /// <summary>The requested categories, or null when no filter was given.</summary>
    /// <exception cref="ValidationException">A value is not a known category.</exception>
    public static IReadOnlySet<ReleaseNoteCategory>? Parse(IEnumerable<string>? values)
    {
        var names = (values ?? [])
            .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToList();

        if (names.Count == 0)
        {
            return null;
        }

        var categories = new HashSet<ReleaseNoteCategory>();
        var unknown = new List<string>();
        foreach (var name in names)
        {
            if (ReleaseNoteCategories.TryParse(name, out var category))
            {
                categories.Add(category);
            }
            else
            {
                unknown.Add(name);
            }
        }

        if (unknown.Count > 0)
        {
            var allowed = string.Join(", ", ReleaseNoteCategories.All.Select(c => c.ToString().ToLowerInvariant()));
            throw new ValidationException(
            [
                new ValidationFailure(
                    ParameterName,
                    $"Unknown category '{string.Join("', '", unknown)}'. Use one or more of: {allowed}."),
            ]);
        }

        return categories;
    }
}
