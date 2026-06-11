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
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor, ct);

        if (business is null)
            throw new NotFoundException("Doctor not found");

        // Cascade delete handles: DoctorProfile, Branches (and their WorkingDays/PhoneNumbers),
        // WorkingDays, PhoneNumbers, BusinessServices, Testimonials, UserFavorites.
        db.Businesses.Remove(business);
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
