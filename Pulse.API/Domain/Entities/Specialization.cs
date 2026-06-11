namespace Pulse.API.Domain.Entities;

public class Specialization : Entity
{
    public string Name { get; set; } = null!;

    public List<DoctorProfile> Doctors { get; set; } = new();
}
