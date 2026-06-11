using MediatR;
using Pulse.API.Domain.Enums;

namespace Pulse.API.Features.Shared.Branches.GetBranches;

public record SharedGetBranchesQuery(Guid BusinessId, BusinessType Type, string? BaseUrl = null) : IRequest<List<BranchListResponse>>;
