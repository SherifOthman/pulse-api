using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.DeleteBranch;

public class SharedDeleteBranchHandler(AppDbContext db)
    : IRequestHandler<SharedDeleteBranchCommand, Unit>
{
    public async Task<Unit> Handle(SharedDeleteBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Businesses
            .FirstOrDefaultAsync(b =>
                b.Id == request.Id &&
                b.Type == request.Type &&
                b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        db.Businesses.Remove(branch);
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
