namespace Pulse.API.Domain.Entities;

public class RefreshToken : Entity
{

    public string TokenHash { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public string? ReplacedByTokenHash { get; set; }

    public string CreatedByIp { get; set; } = null!;
    public string? RevokedByIp { get; set; }

    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;


}
