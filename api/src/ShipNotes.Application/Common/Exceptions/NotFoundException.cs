namespace ShipNotes.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string resource, object key)
        : base($"{resource} '{key}' was not found.")
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
