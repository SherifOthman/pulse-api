namespace Pulse.API.Domain.Entities;

public class Specialization : Entity
{
    public string Name { get; set; } = null!;

    public List<Doctor> Doctors { get; set; } = new();

}
