using Pulse.API.Features.Doctors.Services.GetDoctorServices;

namespace Pulse.API.Features.Doctors.Services.UpdateDoctorServices;

public record UpdateDoctorServicesResponse(List<DoctorServiceResponse> Services);
