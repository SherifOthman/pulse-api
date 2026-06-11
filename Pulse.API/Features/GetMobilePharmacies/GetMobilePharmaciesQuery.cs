using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Pharmacies.GetMobilePharmacies;

public record GetMobilePharmaciesQuery(
    BusinessQuery BusinessQuery,
    string BaseUrl
) : IRequest<PaginatedResponse<PharmacyMobileListResponse>>;
