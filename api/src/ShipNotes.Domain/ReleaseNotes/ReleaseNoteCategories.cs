namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>Display names and parsing for <see cref="ReleaseNoteCategory"/>.</summary>
public static class ReleaseNoteCategories
{
    /// <summary>Every category, in display order.</summary>
    public static IReadOnlyList<ReleaseNoteCategory> All { get; } = Enum.GetValues<ReleaseNoteCategory>();

    public static string Label(this ReleaseNoteCategory category) => category switch
    {
        ReleaseNoteCategory.Breaking => "Breaking changes",
        _ => category.ToString(),
    };

    /// <summary>Parses a category name, ignoring case. Numbers and unknown names are rejected.</summary>
    public static bool TryParse(string? value, out ReleaseNoteCategory category)
    {
        category = default;
        var name = value?.Trim();
        return !string.IsNullOrEmpty(name)
            && char.IsLetter(name[0])
            && Enum.TryParse(name, ignoreCase: true, out category)
            && Enum.IsDefined(category);
    }
}
