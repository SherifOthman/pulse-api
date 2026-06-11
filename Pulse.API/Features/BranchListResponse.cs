namespace Pulse.API.Features.Doctors.Branches;

public record BranchListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? Governorate,
    string? City,
    decimal? VisitPrice,
    bool IsOpen
);
