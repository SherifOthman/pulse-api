using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Radiology.GetMobileRadiology;

public record GetMobileRadiologyQuery(
    BusinessQuery BusinessQuery
) : IRequest<PaginatedResponse<RadiologyMobileListResponse>>;
