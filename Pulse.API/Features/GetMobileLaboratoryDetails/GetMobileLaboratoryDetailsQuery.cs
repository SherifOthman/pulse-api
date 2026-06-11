using MediatR;

namespace Pulse.API.Features.Laboratories.GetMobileLaboratoryDetails;

public record GetMobileLaboratoryDetailsQuery(Guid Id, Guid? CurrentUserId, string BaseUrl)
    : IRequest<LaboratoryMobileDetailsResponse?>;
