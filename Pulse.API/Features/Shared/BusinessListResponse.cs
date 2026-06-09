namespace Pulse.API.Features.Shared;

/// <summary>
/// Base list response shared by all business types (Doctor, Pharmacy, Lab, Radiology).
/// Type-specific responses inherit from this and add their own extra fields.
/// </summary>
public record BusinessListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    int TotalRatings,
    int NextWorkingDay,  // 0=Sunday … 6=Saturday
    string? StartTime,   // "HH:mm"
    string? EndTime,     // "HH:mm"
    bool IsOpen,
    string? CreatedBy = null
);
