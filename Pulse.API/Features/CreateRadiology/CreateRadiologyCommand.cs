using MediatR;

namespace Pulse.API.Features.Radiology.CreateRadiology;

using Pulse.API.Features.Shared;

public record CreateRadiologyCommand(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude,
    List<BusinessServiceItem>? Services = null
) : IRequest<RadiologyResponse>;
