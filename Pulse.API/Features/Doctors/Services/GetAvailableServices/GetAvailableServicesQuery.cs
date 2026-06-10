using MediatR;
using Pulse.API.Features.Doctors.Services.GetDoctorServices;

namespace Pulse.API.Features.Doctors.Services.GetAvailableServices;

/// <summary>Returns all services of BusinessType.Doctor — used to populate the service picker.</summary>
public record GetAvailableServicesQuery : IRequest<List<DoctorServiceResponse>>;
