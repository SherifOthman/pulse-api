using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Specializations.DeleteSpecialization;

public class DeleteSpecializationHandler(AppDbContext db)
    : IRequestHandler<DeleteSpecializationCommand>
{
    public async Task Handle(DeleteSpecializationCommand request, CancellationToken ct)
    {
        var spec = await db.Set<Specialization>().FindAsync([request.Id], ct);
        if (spec is null)
            throw new KeyNotFoundException("Specialization not found");

        db.Set<Specialization>().Remove(spec);
        await db.SaveChangesAsync(ct);
    }
}
