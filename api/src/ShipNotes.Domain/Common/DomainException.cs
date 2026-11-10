namespace ShipNotes.Domain.Common;

/// <summary>Raised when a domain invariant would be broken.</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException()
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
