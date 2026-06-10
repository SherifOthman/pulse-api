namespace Pulse.API.Features.Radiology;

public record RadiologyListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    bool IsOpen,
    string? CreatedBy
);
