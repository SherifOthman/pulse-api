using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Governorates.CreateGovernorate;

public class CreateGovernorateHandler(AppDbContext db)
    : IRequestHandler<CreateGovernorateCommand, GovernorateResponse>
{
    public async Task<GovernorateResponse> Handle(CreateGovernorateCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadHttpRequestException("Name is required");

        var governorate = new Governorate { Name = request.Name.Trim() };
        db.Set<Governorate>().Add(governorate);
        await db.SaveChangesAsync(ct);

        return new GovernorateResponse(governorate.Id, governorate.Name);
    }
}
