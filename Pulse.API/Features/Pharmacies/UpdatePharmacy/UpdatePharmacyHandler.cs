using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
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
            throw new KeyNotFoundException("Pharmacy not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            business.Name = request.Name.Trim();

        if (request.CityId.HasValue)
        {
            if (!await db.Set<Domain.Entities.City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
                throw new BadHttpRequestException("City not found");
            business.CityId = request.CityId.Value;
        }

        if (request.Address is not null)
            business.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.Description is not null)
            business.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        if (request.ProfileImageUrl is not null)
            business.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();

        if (request.CoverImageUrl is not null)
            business.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();

        await db.SaveChangesAsync(ct);
        return new PharmacyResponse(business.Id, business.Name);
    }
}
