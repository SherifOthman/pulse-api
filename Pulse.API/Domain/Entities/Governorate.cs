namespace Pulse.API.Domain.Entities;

public class Governorate : Entity
{
    public string Name { get; set; } = null!;

    public List<City> Cities { get; set; } = new List<City>();
}
