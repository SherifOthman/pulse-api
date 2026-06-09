using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Laboratories;

/// <summary>
/// Laboratory list response — no extra fields beyond the shared base.
/// </summary>
public record LaboratoryListResponse(
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
    string? CreatedBy = null
) : BusinessListResponse(Id, Name, ProfileImageUrl, Governorate,
    AverageRating, TotalRatings, NextWorkingDay, StartTime, EndTime, IsOpen, CreatedBy);
