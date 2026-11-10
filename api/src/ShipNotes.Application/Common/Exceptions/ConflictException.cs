namespace ShipNotes.Application.Common.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException()
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
