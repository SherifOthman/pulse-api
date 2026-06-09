using MediatR;

namespace Pulse.API.Features.Specializations.CreateSpecialization;

public record CreateSpecializationCommand(string Name) : IRequest<SpecializationResponse>;
