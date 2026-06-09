using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetMobileLaboratories;

public record GetMobileLaboratoriesQuery(
    BusinessQuery BusinessQuery
) : IRequest<PaginatedResponse<LaboratoryMobileListResponse>>;
