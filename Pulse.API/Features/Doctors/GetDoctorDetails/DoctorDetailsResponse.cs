using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.GetDoctorDetails;

public record DoctorDetailsResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    string? Description,
    string? Address,
    string Governorate,
    Guid GovernorateId,
    string City,
    Guid CityId,
    double? Latitude,
    double? Longitude,
    double AverageRating,
    int TotalRatings,
    int Gender,          // 0=Male, 1=Female
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services,
    string Specialization,
    decimal? VisitPrice
);
