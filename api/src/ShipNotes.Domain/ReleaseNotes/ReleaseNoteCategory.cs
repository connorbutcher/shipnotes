namespace ShipNotes.Domain.ReleaseNotes;

/// <summary>
/// The kind of change a section of a release note describes, following Keep a Changelog
/// with breaking changes called out first. Declaration order is display order.
/// </summary>
public enum ReleaseNoteCategory
{
    Breaking = 0,
    Added = 1,
    Changed = 2,
    Deprecated = 3,
    Removed = 4,
    Fixed = 5,
    Security = 6,
}
