using MediatR;

namespace Pulse.API.Features.Radiology.GetRadiologyDetails;

public record GetRadiologyDetailsQuery(Guid Id) : IRequest<RadiologyDetailsResponse?>;
