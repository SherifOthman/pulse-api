namespace Pulse.API.Features.Laboratories;

public record LaboratoryListResponse(
    Guid Id,
    string Name,
    string Governorate,
    double AverageRating,
    string? CreatedBy
);
