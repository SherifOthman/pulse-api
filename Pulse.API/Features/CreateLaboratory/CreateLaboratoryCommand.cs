using MediatR;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

using Pulse.API.Features.Shared;

public record CreateLaboratoryCommand(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude,
    List<BusinessServiceItem>? Services = null
) : IRequest<LabResponse>;
