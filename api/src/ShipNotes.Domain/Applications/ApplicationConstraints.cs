namespace ShipNotes.Domain.Applications;

public static class ApplicationConstraints
{
    public const int NameMaxLength = 100;
    public const int IconUrlMaxLength = 2048;
    public const int ApiKeyHashLength = 64;
    public const int ApiKeyPrefixLength = 12;
}
