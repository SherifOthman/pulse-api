namespace Pulse.API.Features.Pharmacies;

public record PharmacyListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    bool IsOpen,
    string? CreatedBy
);
