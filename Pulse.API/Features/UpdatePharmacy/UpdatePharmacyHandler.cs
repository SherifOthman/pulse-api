using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
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
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Pharmacy, ct);

        if (business is null)
            throw new NotFoundException("Pharmacy not found");

        await BusinessMappingHelpers.ApplyUpdatesAsync(
            db, business,
            request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude, ct);

        if (request.WorkingDays is not null)
        {
            db.Set<WorkingDay>().RemoveRange(business.WorkingDays);
            var newDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
            foreach (var d in newDays) { d.BusinessId = business.Id; db.Set<WorkingDay>().Add(d); }
        }

        if (request.PhoneNumbers is not null)
        {
            db.Set<PhoneNumber>().RemoveRange(business.PhoneNumbers);
            var newNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);
            foreach (var p in newNumbers) { p.BusinessId = business.Id; db.Set<PhoneNumber>().Add(p); }
        }

        await db.SaveChangesAsync(ct);
        return new PharmacyResponse(business.Id, business.Name);
    }
}
