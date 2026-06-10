namespace Pulse.API.Features.Laboratories;

public record LaboratoryListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Governorate,
    double AverageRating,
    bool IsOpen,
    string? CreatedBy
);
