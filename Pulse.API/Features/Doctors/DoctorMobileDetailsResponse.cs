using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors;

/// <summary>
/// Mobile app detail response for a doctor.
/// Includes IsFavorite / HasUserReviewed for the authenticated mobile user.
/// </summary>
public record DoctorMobileDetailsResponse(
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
    List<ServiceDto> Services,
    string Specialization,
    decimal? VisitPrice
);
