using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Laboratories;

public record LaboratoryMobileDetailsResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    string? Description,
    string? Address,
    string Governorate,
    string City,
    double AverageRating,
    int TotalRatings,
    bool IsFavorite,
    bool HasUserReviewed,
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services
);
