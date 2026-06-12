using MediatR;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Pharmacies.CreatePharmacy;

public class CreatePharmacyHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreatePharmacyCommand, PharmacyResponse>
{
    public async Task<PharmacyResponse> Handle(CreatePharmacyCommand request, CancellationToken ct)
    {
        var business = await BusinessMappingHelpers.CreateBusinessAsync(
            db,
            request.Name,
            request.CityId,
            request.Address,
            request.Description,
            request.ProfileImageUrl,
            request.CoverImageUrl,
            request.Latitude,
            request.Longitude,
            BusinessType.Pharmacy,
            currentUser.Id,
            ct);

        business.PharmacyProfile = new PharmacyProfile();
        business.WorkingDays     = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
        business.PhoneNumbers    = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        if (request.Services is { Count: > 0 })
            await BusinessServiceHelpers.LinkServicesAsync(
                db, business.Id, BusinessType.Pharmacy, request.Services, ct);

        return new PharmacyResponse(business.Id, business.Name);
    }
}
