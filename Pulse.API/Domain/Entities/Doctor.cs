using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

/// <summary>
/// Doctor-specific profile. Linked 1:1 to the parent Business via BusinessId (PK = FK).
/// </summary>
public class DoctorProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>Legacy single specialization — kept for mobile backward-compat, derived at query time.</summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Default visit price for the main/only location.
    /// Each branch can override this with its own VisitPrice.
    /// </summary>
    public decimal? VisitPrice { get; set; }

    public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
}
