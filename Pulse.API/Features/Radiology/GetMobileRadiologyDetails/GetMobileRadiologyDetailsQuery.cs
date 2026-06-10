using MediatR;

namespace Pulse.API.Features.Radiology.GetMobileRadiologyDetails;

public record GetMobileRadiologyDetailsQuery(Guid Id, Guid? CurrentUserId, string BaseUrl)
    : IRequest<RadiologyMobileDetailsResponse?>;
