using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Pharmacies.UpdatePharmacy;

public class UpdatePharmacyHandler(AppDbContext db)
    : IRequestHandler<UpdatePharmacyCommand, PharmacyResponse>
{
    public async Task<PharmacyResponse> Handle(UpdatePharmacyCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Pharmacy, ct);

        if (business is null)
            throw new NotFoundException("Pharmacy not found");

        await BusinessMappingHelpers.ApplyUpdatesAsync(
            db, business,
            request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude, ct);

        await db.SaveChangesAsync(ct);
        return new PharmacyResponse(business.Id, business.Name);
    }
}
