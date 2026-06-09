using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Pharmacies.GetPharmacyDetails;

public record PharmacyDetailsResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    string? Description,
    string? Address,
    string Governorate,
    string City,
    double? Latitude,
    double? Longitude,
    double AverageRating,
    int TotalRatings,
    bool IsFavorite,
    bool HasUserReviewed,
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services
) : BusinessDetailsResponse(Id, Name, ProfileImageUrl, CoverImageUrl, Description, Address,
    Governorate, City, Latitude, Longitude, AverageRating, TotalRatings, IsFavorite,
    HasUserReviewed, WorkingDays, PhoneNumbers, Branches, Testimonials, Services);
