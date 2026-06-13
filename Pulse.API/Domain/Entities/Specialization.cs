namespace Pulse.API.Domain.Entities;

public class Specialization : Entity
{
    public string Name { get; set; } = null!;

    public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
}
