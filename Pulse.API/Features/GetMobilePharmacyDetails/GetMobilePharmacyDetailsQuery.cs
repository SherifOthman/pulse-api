using MediatR;

namespace Pulse.API.Features.Pharmacies.GetMobilePharmacyDetails;

public record GetMobilePharmacyDetailsQuery(Guid Id, Guid? CurrentUserId, string BaseUrl)
    : IRequest<PharmacyMobileDetailsResponse?>;
