namespace Pulse.API.Features.Radiology;

public record RadiologyListResponse(
    Guid Id,
    string Name,
    string Governorate,
    double AverageRating,
    string? CreatedBy
);
