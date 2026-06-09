using MediatR;

namespace Pulse.API.Features.Laboratories.GetLaboratoryDetails;

public record GetLaboratoryDetailsQuery(Guid Id) : IRequest<LaboratoryDetailsResponse?>;
