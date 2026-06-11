using MediatR;

namespace Pulse.API.Features.Pharmacies.GetPharmacyDetails;

public record GetPharmacyDetailsQuery(Guid Id) : IRequest<PharmacyDetailsResponse?>;
