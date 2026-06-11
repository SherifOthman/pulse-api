using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.CreateBranch;

public class SharedCreateBranchHandler(AppDbContext db)
    : IRequestHandler<SharedCreateBranchCommand, CreateBranchResponse>
{
    public async Task<CreateBranchResponse> Handle(SharedCreateBranchCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Branch name is required");

        var parent = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.BusinessId && b.Type == request.Type, ct);

        if (parent is null)
            throw new NotFoundException("Parent business not found");

        var resolvedCityId = request.CityId ?? parent.CityId;

        if (!await db.Set<City>().AnyAsync(c => c.Id == resolvedCityId, ct))
            throw new NotFoundException("City not found");

        var branch = new Branch
        {
            ParentBusinessId = request.BusinessId,
            Name             = request.Name.Trim(),
            CityId           = resolvedCityId,
            Address          = request.Address?.Trim(),
            Latitude         = request.Latitude,
            Longitude        = request.Longitude,
            // VisitPrice is null for non-doctor types
            WorkingDays  = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays),
            PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers),
        };

        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);

        return new CreateBranchResponse(branch.Id, branch.Name);
    }
}
