using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

public class Business : Entity
{
    // ── Core identity fields (parent only — never duplicated on branches) ──────
    public string Name { get; set; } = null!;
    public BusinessType Type { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Description { get; set; }

    // ── Location fields (also on branches, each branch has its own) ───────────
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    // ── Per-location contact & schedule ───────────────────────────────────────
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    public List<WorkingDay> WorkingDays { get; set; } = new();

    // ── Parent-only data ──────────────────────────────────────────────────────
    public List<BusinessService> BusinessServices { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = new();
    public List<Branch> Branches { get; set; } = new();

    // ── Type-specific profiles (1:1, only one non-null based on Type) ─────────
    public DoctorProfile? DoctorProfile { get; set; }
    public PharmacyProfile? PharmacyProfile { get; set; }
    public LaboratoryProfile? LaboratoryProfile { get; set; }
    public RadiologyProfile? RadiologyProfile { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }
}
