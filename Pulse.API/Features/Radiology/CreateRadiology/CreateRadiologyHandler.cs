using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Radiology.CreateRadiology;

public class CreateRadiologyHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateRadiologyCommand, RadiologyResponse>
{
    public async Task<RadiologyResponse> Handle(CreateRadiologyCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadHttpRequestException("Name is required");

        if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId, ct))
            throw new BadHttpRequestException("City not found");

        var business = new Business
        {
            Name = request.Name.Trim(),
            Type = BusinessType.Radiology,
            CityId = request.CityId,
            Address = request.Address?.Trim(),
            Description = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            CreatedByUserId = currentUser.Id,
        };

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        return new RadiologyResponse(business.Id, business.Name);
    }
}
