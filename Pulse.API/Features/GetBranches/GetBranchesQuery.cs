using MediatR;

namespace Pulse.API.Features.Doctors.Branches.GetBranches;

public record GetBranchesQuery(Guid DoctorId, string? BaseUrl = null) : IRequest<List<BranchListResponse>>;
