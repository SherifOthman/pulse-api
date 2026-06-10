using MediatR;

namespace Pulse.API.Features.Doctors.Branches.DeleteBranch;

public record DeleteBranchCommand(Guid Id) : IRequest<Unit>;
