using MediatR;

namespace Pulse.API.Features.Radiology.DeleteRadiology;

public record DeleteRadiologyCommand(Guid Id) : IRequest<Unit>;
