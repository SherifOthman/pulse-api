namespace Pulse.API.Domain.Entities;

/// <summary>
/// A physical branch location of a parent Business.
/// Branches hold only location-specific data. All identity data
/// (name override, description, images, services, testimonials)
/// belongs to the parent Business.
/// </summary>
public class Branch : Entity
{
    public Guid ParentBusinessId { get; set; }
    public Business ParentBusiness { get; set; } = null!;

    /// <summary>Branch display name (e.g. "Cairo Branch").</summary>
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    /// <summary>
    /// Doctor-type branches only. The visit price for this specific location.
    /// Null for all other business types.
    /// </summary>  
    public decimal? VisitPrice { get; set; }

    // Per-branch contact and schedule
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    public List<WorkingDay> WorkingDays { get; set; } = new();
}
