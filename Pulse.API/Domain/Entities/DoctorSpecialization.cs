namespace Pulse.API.Domain.Entities;

/// <summary>
/// Join table for the many-to-many relationship between DoctorProfile and Specialization.
/// </summary>
public class DoctorSpecialization
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;

    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
}
