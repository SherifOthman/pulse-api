using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

/// <summary>
/// Doctor-specific profile. Linked 1:1 to the parent Business via BusinessId (PK = FK).
/// VisitPrice is per-branch (see Branch.VisitPrice), not stored here.
/// </summary>
public class DoctorProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
    public Gender Gender { get; set; }
}
