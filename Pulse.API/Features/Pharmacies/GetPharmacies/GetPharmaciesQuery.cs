using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Pharmacies.GetPharmacies;

public record GetPharmaciesQuery(
    BusinessQuery BusinessQuery
) : IRequest<PaginatedResponse<PharmacyListResponse>>;
