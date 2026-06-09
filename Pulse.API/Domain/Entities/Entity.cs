namespace Pulse.API.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
