namespace Pulse.API.Features.Radiology;

public record RadiologyMobileListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    int TotalRatings,
    int NextWorkingDay,
    string? StartTime,
    string? EndTime,
    bool IsOpen
);
