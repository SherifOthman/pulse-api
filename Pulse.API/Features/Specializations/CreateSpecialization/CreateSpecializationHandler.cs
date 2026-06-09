using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Specializations.CreateSpecialization;

public class CreateSpecializationHandler(AppDbContext db)
    : IRequestHandler<CreateSpecializationCommand, SpecializationResponse>
{
    public async Task<SpecializationResponse> Handle(CreateSpecializationCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadHttpRequestException("Name is required");

        var spec = new Specialization { Name = request.Name.Trim() };
        db.Set<Specialization>().Add(spec);
        await db.SaveChangesAsync(ct);

        return new SpecializationResponse(spec.Id, spec.Name);
    }
}
