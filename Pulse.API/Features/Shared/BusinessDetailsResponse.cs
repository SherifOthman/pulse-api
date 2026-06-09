namespace Pulse.API.Features.Shared;

/// <summary>
/// Base detail response shared by all business types.
/// Type-specific responses inherit from this and add their own extra fields.
/// </summary>
public record BusinessDetailsResponse(
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
);
