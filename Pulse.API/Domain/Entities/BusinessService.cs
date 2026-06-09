namespace Pulse.API.Domain.Entities;

public class BusinessService
{
    public Guid BusinessId { get; set; }

    public Business Business { get; set; } = null!;

    public Guid ServiceId { get; set; }

    public Service Service { get; set; } = null!;


}
