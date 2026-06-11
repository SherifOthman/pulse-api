using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Specializations.UpdateSpecialization;

public class UpdateSpecializationHandler(AppDbContext db)
    : IRequestHandler<UpdateSpecializationCommand, SpecializationResponse>
{
    public async Task<SpecializationResponse> Handle(UpdateSpecializationCommand request, CancellationToken ct)
    {
        var spec = await db.Set<Specialization>().FindAsync([request.Id], ct);
        if (spec is null)
            throw new KeyNotFoundException("Specialization not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            spec.Name = request.Name.Trim();

        await db.SaveChangesAsync(ct);
        return new SpecializationResponse(spec.Id, spec.Name);
    }
}
