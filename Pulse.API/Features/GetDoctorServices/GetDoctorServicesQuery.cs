using MediatR;

namespace Pulse.API.Features.Doctors.Services.GetDoctorServices;

/// <summary>Returns the list of services currently linked to a doctor.</summary>
public record GetDoctorServicesQuery(Guid DoctorId) : IRequest<List<DoctorServiceResponse>>;
