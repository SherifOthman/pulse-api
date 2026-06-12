using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

/// <summary>
/// Doctor-specific profile. Linked 1:1 to the parent Business via BusinessId (PK = FK).
/// </summary>
public class DoctorProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
    public Gender Gender { get; set; }

    /// <summary>
    /// Default visit price for the main/only location.
    /// Each branch can override this with its own VisitPrice.
    /// </summary>
    public decimal? VisitPrice { get; set; }
}
