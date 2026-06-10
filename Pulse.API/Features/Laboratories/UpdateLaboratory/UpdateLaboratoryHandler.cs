using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Laboratories.UpdateLaboratory;

public class UpdateLaboratoryHandler(AppDbContext db)
    : IRequestHandler<UpdateLaboratoryCommand, LabResponse>
{
    public async Task<LabResponse> Handle(UpdateLaboratoryCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Laboratory, ct);

        if (business is null)
            throw new NotFoundException("Lab not found");

        await BusinessMappingHelpers.ApplyUpdatesAsync(
            db, business, request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude, ct);

        await db.SaveChangesAsync(ct);
        return new LabResponse(business.Id, business.Name);
    }
}
