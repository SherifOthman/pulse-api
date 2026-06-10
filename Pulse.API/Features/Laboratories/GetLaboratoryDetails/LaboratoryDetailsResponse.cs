using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Laboratories.GetLaboratoryDetails;

/// <summary>Dashboard detail response — no IsFavorite/HasUserReviewed (mobile-only).</summary>
public record LaboratoryDetailsResponse(
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
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services
);
