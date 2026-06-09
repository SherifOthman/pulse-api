using MediatR;

namespace Pulse.API.Features.Laboratories.DeleteLaboratory;

public record DeleteLaboratoryCommand(Guid Id) : IRequest<Unit>;
