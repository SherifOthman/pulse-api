using MediatR;

namespace Pulse.API.Features.Doctors.GetDoctorDetails;

public record GetDoctorDetailsQuery(Guid Id) : IRequest<DoctorDetailsResponse?>;
