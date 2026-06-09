using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Radiology.GetRadiology;

public record GetRadiologyQuery(
    BusinessQuery BusinessQuery
) : IRequest<PaginatedResponse<RadiologyListResponse>>;
