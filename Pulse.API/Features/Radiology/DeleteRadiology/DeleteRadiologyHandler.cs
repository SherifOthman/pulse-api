using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Radiology.DeleteRadiology;

public class DeleteRadiologyHandler(AppDbContext db)
    : IRequestHandler<DeleteRadiologyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRadiologyCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Radiology, ct);

        if (business is null)
            throw new NotFoundException("Radiology center not found");

        db.Businesses.Remove(business);
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
