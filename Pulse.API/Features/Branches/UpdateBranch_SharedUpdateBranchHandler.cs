using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.UpdateBranch;

public class SharedUpdateBranchHandler(AppDbContext db)
    : IRequestHandler<SharedUpdateBranchCommand, UpdateBranchResponse>
{
    public async Task<UpdateBranchResponse> Handle(SharedUpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Branches
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b =>
                b.Id == request.Id &&
                b.ParentBusiness.Type == request.Type, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            branch.Name = request.Name.Trim();

        if (request.CityId.HasValue)    branch.CityId    = request.CityId.Value;
        if (request.Latitude.HasValue)  branch.Latitude  = request.Latitude;
        if (request.Longitude.HasValue) branch.Longitude = request.Longitude;

        if (request.Address is not null)
            branch.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.WorkingDays is not null)
        {
            db.Set<WorkingDay>().RemoveRange(branch.WorkingDays);
            await db.SaveChangesAsync(ct);
            var newDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
            foreach (var day in newDays)
                branch.WorkingDays.Add(day);
        }

        if (request.PhoneNumbers is not null)
        {
            db.Set<PhoneNumber>().RemoveRange(branch.PhoneNumbers);
            await db.SaveChangesAsync(ct);
            var newNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);
            foreach (var phone in newNumbers)
                branch.PhoneNumbers.Add(phone);
        }

        await db.SaveChangesAsync(ct);
        return new UpdateBranchResponse(branch.Id, branch.Name);
    }
}
