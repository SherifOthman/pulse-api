using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.UpdateBranch;

public class SharedUpdateBranchHandler(AppDbContext db)
    : IRequestHandler<SharedUpdateBranchCommand, UpdateBranchResponse>
{
    public async Task<UpdateBranchResponse> Handle(SharedUpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Businesses
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b =>
                b.Id == request.Id &&
                b.Type == request.Type &&
                b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            branch.Name = request.Name.Trim();

        if (request.CityId.HasValue)    branch.CityId = request.CityId.Value;
        if (request.Latitude.HasValue)  branch.Latitude  = request.Latitude;
        if (request.Longitude.HasValue) branch.Longitude = request.Longitude;

        if (request.Address is not null)
            branch.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.WorkingDays is not null)
            branch.WorkingDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);

        if (request.PhoneNumbers is not null)
            branch.PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);

        await db.SaveChangesAsync(ct);
        return new UpdateBranchResponse(branch.Id, branch.Name);
    }
}
