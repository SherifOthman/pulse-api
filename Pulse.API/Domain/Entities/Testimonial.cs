namespace Pulse.API.Domain.Entities;

public class Testimonial : Entity
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string Text { get; set; } = null!;
    public byte Rating { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

}
