using MediatR;

namespace Pulse.API.Features.Doctors.Branches.GetBranches;

public record GetBranchesQuery(Guid DoctorId) : IRequest<List<BranchListResponse>>;
