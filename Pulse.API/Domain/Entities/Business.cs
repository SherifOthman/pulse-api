using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

public class Business : Entity
{
    public string Name { get; set; } = null!;
    public BusinessType Type { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? WorkingHours { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid? ParentBusinessId { get; set; }
    public Business? ParentBusiness { get; set; }
    public List<Business> Branches { get; set; } = new();

    public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    public List<BusinessService> BusinessServices { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = new();

    public List<WorkingDay> WorkingDays { get; set; } = new();

    public Guid CityId { get; set; }

    public City City  { get; set;} = null!;



    public Doctor? Doctor { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }
}
