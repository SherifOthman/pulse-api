namespace Pulse.API.Features.Doctors;

public record DoctorMobileListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    int TotalRatings,
    int NextWorkingDay,
    string? StartTime,
    string? EndTime,
    bool IsOpen,
    string Specialization
);
