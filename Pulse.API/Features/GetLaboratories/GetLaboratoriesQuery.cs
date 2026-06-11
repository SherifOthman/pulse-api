using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetLaboratories;

public record GetLaboratoriesQuery(
    BusinessQuery BusinessQuery
) : IRequest<PaginatedResponse<LaboratoryListResponse>>;
