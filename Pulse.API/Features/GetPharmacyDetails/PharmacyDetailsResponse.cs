using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Pharmacies.GetPharmacyDetails;

/// <summary>
/// Dashboard detail response for a pharmacy.
/// Does NOT include IsFavorite / HasUserReviewed — those are mobile-only.
/// </summary>
public record PharmacyDetailsResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    string? Description,
    string? Address,
    string Governorate,
    string City,
    Guid? GovernorateId,
    Guid? CityId,
    double? Latitude,
    double? Longitude,
    double AverageRating,
    int TotalRatings,
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services
);
