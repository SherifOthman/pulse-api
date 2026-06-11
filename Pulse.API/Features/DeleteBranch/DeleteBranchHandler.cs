using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.DeleteBranch;

public class DeleteBranchHandler(AppDbContext db)
    : IRequestHandler<DeleteBranchCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Branches
            .FirstOrDefaultAsync(b =>
                b.Id == request.Id &&
                b.ParentBusiness.Type == BusinessType.Doctor, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        // Cascade handles WorkingDays and PhoneNumbers via Branch FK
        db.Branches.Remove(branch);
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
