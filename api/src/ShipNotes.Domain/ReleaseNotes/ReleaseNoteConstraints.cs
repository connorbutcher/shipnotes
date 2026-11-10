namespace ShipNotes.Domain.ReleaseNotes;

public static class ReleaseNoteConstraints
{
    public const int TitleMaxLength = 200;
    public const int ContentMaxLength = 200_000;
    public const int SortKeyMaxLength = 400;
    public const int CategoryMaxLength = 20;
}
