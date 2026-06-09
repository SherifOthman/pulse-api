using MediatR;

namespace Pulse.API.Features.Specializations.UpdateSpecialization;

public record UpdateSpecializationCommand(Guid Id, string? Name) : IRequest<SpecializationResponse>;
