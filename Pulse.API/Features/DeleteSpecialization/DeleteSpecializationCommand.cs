using MediatR;

namespace Pulse.API.Features.Specializations.DeleteSpecialization;

public record DeleteSpecializationCommand(Guid Id) : IRequest;
