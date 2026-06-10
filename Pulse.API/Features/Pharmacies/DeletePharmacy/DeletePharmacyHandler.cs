using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Pharmacies.DeletePharmacy;

public class DeletePharmacyHandler(AppDbContext db)
    : IRequestHandler<DeletePharmacyCommand, Unit>
{
    public async Task<Unit> Handle(DeletePharmacyCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Pharmacy, ct);

        if (business is null)
            throw new NotFoundException("Pharmacy not found");

        db.Businesses.Remove(business);
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
