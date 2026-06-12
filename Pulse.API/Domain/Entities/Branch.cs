namespace Pulse.API.Domain.Entities;

/// <summary>
/// A physical branch location of a parent Business.
/// Branches hold only location-specific data shared across all types.
/// Type-specific data lives in extension tables (e.g. DoctorBranchProfile).
/// </summary>
public class Branch : Entity
{
    public Guid ParentBusinessId { get; set; }
    public Business ParentBusiness { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    public List<WorkingDay> WorkingDays { get; set; } = new();

    /// <summary>Only populated for Doctor branches.</summary>
    public DoctorBranchProfile? DoctorBranchProfile { get; set; }
}
