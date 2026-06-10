using MediatR;

namespace Pulse.API.Features.Radiology.UpdateRadiology;

public record UpdateRadiologyCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude
) : IRequest<RadiologyResponse>;
