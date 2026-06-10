using MediatR;

namespace Pulse.API.Features.Doctors.Branches.GetBranchDetails;

public record GetBranchDetailsQuery(Guid Id) : IRequest<BranchDetailsResponse?>;
