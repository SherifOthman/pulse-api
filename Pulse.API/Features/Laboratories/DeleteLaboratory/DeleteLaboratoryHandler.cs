using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Laboratories.DeleteLaboratory;

public class DeleteLaboratoryHandler(AppDbContext db)
    : IRequestHandler<DeleteLaboratoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteLaboratoryCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Laboratory, ct);

        if (business is null)
            throw new KeyNotFoundException("Lab not found");

        db.Businesses.Remove(business);
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
