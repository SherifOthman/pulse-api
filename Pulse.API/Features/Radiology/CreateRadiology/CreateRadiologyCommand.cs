using MediatR;

namespace Pulse.API.Features.Radiology.CreateRadiology;

public record CreateRadiologyCommand(
    string Name,
    Guid CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl
) : IRequest<RadiologyResponse>;
