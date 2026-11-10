namespace ShipNotes.Domain.Common;

/// <summary>Base type for persisted aggregates with audit timestamps.</summary>
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.CreateVersion7();

    public DateTimeOffset CreatedAt { get; protected set; }

    public DateTimeOffset UpdatedAt { get; protected set; }

    protected void Touch(DateTimeOffset now)
    {
        if (CreatedAt == default)
        {
            CreatedAt = now;
        }

        UpdatedAt = now;
    }
}
