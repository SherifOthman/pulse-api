using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.DeleteDoctor;

public class DeleteDoctorHandler(AppDbContext db)
    : IRequestHandler<DeleteDoctorCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDoctorCommand request, CancellationToken ct)
    {
        // Load the main doctor business + its branches (children)
        var business = await db.Businesses
            .Include(b => b.Doctor)
            .Include(b => b.Branches)
                .ThenInclude(br => br.Doctor)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor, ct);

        if (business is null)
            throw new NotFoundException("Doctor not found");

        // Remove branch Doctor records first (owned table, no cascade configured)
        foreach (var branch in business.Branches)
        {
            if (branch.Doctor is not null)
                db.Set<Doctor>().Remove(branch.Doctor);
        }

        // Remove all child branches
        db.Businesses.RemoveRange(business.Branches);

        // Remove the main Doctor record
        if (business.Doctor is not null)
            db.Set<Doctor>().Remove(business.Doctor);

        // Remove the main Business record
        db.Businesses.Remove(business);

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
