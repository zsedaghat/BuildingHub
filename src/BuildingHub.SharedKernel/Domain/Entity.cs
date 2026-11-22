namespace BuildingHub.SharedKernel.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; private set; }

    protected Entity()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
    }
}