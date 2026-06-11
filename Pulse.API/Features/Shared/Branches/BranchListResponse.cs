namespace Pulse.API.Features.Shared.Branches;

public record BranchListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string? Governorate,
    string? City,
    bool IsOpen
);
