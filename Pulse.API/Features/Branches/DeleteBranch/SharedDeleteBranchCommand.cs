using MediatR;
using Pulse.API.Domain.Enums;

namespace Pulse.API.Features.Shared.Branches.DeleteBranch;

public record SharedDeleteBranchCommand(Guid Id, BusinessType Type) : IRequest<Unit>;
