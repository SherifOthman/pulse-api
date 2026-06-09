using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors;

/// <summary>
/// Doctor-specific list response — adds Specialization and VisitPrice to the shared base.
/// </summary>
public record DoctorListResponse(
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
    // Doctor-specific
    string Specialization,
    decimal? VisitPrice,
    string? CreatedBy = null
) : BusinessListResponse(Id, Name, ProfileImageUrl, Governorate,
    AverageRating, TotalRatings, NextWorkingDay, StartTime, EndTime, IsOpen, CreatedBy);
