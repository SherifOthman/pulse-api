namespace Pulse.API.Domain.Entities;

public class PhoneNumber : Entity
{
    // Exactly one of these two will be set — the other will be null.
    public Guid? BusinessId { get; set; }
    public Business? Business { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public string? Type { get; set; }
    public string Number { get; set; } = null!;
}
