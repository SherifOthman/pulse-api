using MediatR;

namespace Pulse.API.Features.Doctors.DeleteDoctor;

public record DeleteDoctorCommand(Guid Id) : IRequest<Unit>;
