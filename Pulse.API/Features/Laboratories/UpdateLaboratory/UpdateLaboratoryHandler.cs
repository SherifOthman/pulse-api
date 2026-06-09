using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Laboratories.UpdateLaboratory;

public class UpdateLaboratoryHandler(AppDbContext db)
    : IRequestHandler<UpdateLaboratoryCommand, LabResponse>
{
    public async Task<LabResponse> Handle(UpdateLaboratoryCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Laboratory, ct);

        if (business is null)
            throw new KeyNotFoundException("Lab not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            business.Name = request.Name.Trim();

        if (request.CityId.HasValue)
        {
            if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
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

        return new LabResponse(business.Id, business.Name);
    }
}
