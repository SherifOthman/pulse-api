using MediatR;

namespace Pulse.API.Features.Doctors.GetMobileDoctorDetails;

public record GetMobileDoctorDetailsQuery(Guid Id, Guid? CurrentUserId, string BaseUrl)
    : IRequest<DoctorMobileDetailsResponse?>;
