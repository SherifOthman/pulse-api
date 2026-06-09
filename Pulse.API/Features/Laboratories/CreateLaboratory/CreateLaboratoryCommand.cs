using MediatR;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public record CreateLaboratoryCommand(
    string Name,
    Guid CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl
) : IRequest<LabResponse>;
