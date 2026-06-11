namespace Pulse.API.Domain.Entities;

public class UserFavorite
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

}
