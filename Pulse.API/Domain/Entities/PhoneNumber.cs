namespace Pulse.API.Domain.Entities;

public class PhoneNumber
{
    public Guid Id { get; set; }

    public Guid BusinessId { get; set; }

    public Business Business { get; set; } = null!;

    public string? Type { get; set; }

    public string Number { get; set; } = null!;
}
