using MediatR;
using Pulse.API.Domain.Enums;

namespace Pulse.API.Features.Shared.Branches.GetBranchDetails;

public record SharedGetBranchDetailsQuery(Guid Id, BusinessType Type) : IRequest<BranchDetailsResponse?>;
