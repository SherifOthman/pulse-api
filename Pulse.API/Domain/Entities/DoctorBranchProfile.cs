namespace Pulse.API.Domain.Entities;

/// <summary>
/// Doctor-specific branch data. Extension table for Branch,
/// only exists for branches whose parent business is a Doctor.
/// Keeps the shared Branch table clean and makes it easy to add
/// more doctor-branch-specific fields here in the future.
/// </summary>
public class DoctorBranchProfile
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    /// <summary>Consultation fee at this branch location.</summary>
    public decimal? VisitPrice { get; set; }
}
