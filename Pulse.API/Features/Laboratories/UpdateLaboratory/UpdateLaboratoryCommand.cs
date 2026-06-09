using MediatR;

namespace Pulse.API.Features.Laboratories.UpdateLaboratory;

public record UpdateLaboratoryCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl
) : IRequest<LabResponse>;
