using MediatR;

namespace Pulse.API.Features.Pharmacies.DeletePharmacy;

public record DeletePharmacyCommand(Guid Id) : IRequest<Unit>;
