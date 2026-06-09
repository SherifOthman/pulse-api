namespace Pulse.API.Domain.Entities;

public class City : Entity
{
    public Guid GovernorateId { get; set;}

    public Governorate Governorate { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<Business> Businesses { get; set; } = new();

}
