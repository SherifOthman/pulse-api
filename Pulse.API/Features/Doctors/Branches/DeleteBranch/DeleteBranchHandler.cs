using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.DeleteBranch;

public class DeleteBranchHandler(AppDbContext db)
    : IRequestHandler<DeleteBranchCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Businesses
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor && b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (branch.Doctor is not null)
            db.Set<Doctor>().Remove(branch.Doctor);

        db.Businesses.Remove(branch);
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
