namespace Pulse.API.Domain.Entities;

public class UserFavorite
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid BuissnessId { get; set; }
    public Business Buissness { get; set; } = null!;

}
