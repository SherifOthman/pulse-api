using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public class CreateLaboratoryHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateLaboratoryCommand, LabResponse>
{
    public async Task<LabResponse> Handle(CreateLaboratoryCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadHttpRequestException("Name is required");

        if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId, ct))
            throw new BadHttpRequestException("City not found");

        var business = new Business
        {
            Name = request.Name.Trim(),
            Type = BusinessType.Laboratory,
            CityId = request.CityId,
            Address = request.Address?.Trim(),
            Description = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            CreatedByUserId = currentUser.Id,
        };

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        return new LabResponse(business.Id, business.Name);
    }
}
