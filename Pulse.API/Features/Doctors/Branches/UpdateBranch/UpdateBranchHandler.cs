using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.UpdateBranch;

public class UpdateBranchHandler(AppDbContext db)
    : IRequestHandler<UpdateBranchCommand, UpdateBranchResponse>
{
    public async Task<UpdateBranchResponse> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Businesses
            .Include(b => b.Doctor)
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b =>
                b.Id == request.Id &&
                b.Type == BusinessType.Doctor &&
                b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            branch.Name = request.Name.Trim();

        if (request.CityId.HasValue)    branch.CityId    = request.CityId.Value;
        if (request.Latitude.HasValue)  branch.Latitude  = request.Latitude;
        if (request.Longitude.HasValue) branch.Longitude = request.Longitude;

        if (request.Address is not null)
            branch.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (branch.Doctor is not null && request.VisitPrice.HasValue)
            branch.Doctor.VisitPrice = request.VisitPrice;

        if (request.WorkingDays is not null)
        {
            db.Set<WorkingDay>().RemoveRange(branch.WorkingDays);
            await db.SaveChangesAsync(ct);
            var newDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
            foreach (var day in newDays)
            {
                day.BusinessId = branch.Id;
                branch.WorkingDays.Add(day);
            }
        }

        if (request.PhoneNumbers is not null)
        {
            db.Set<PhoneNumber>().RemoveRange(branch.PhoneNumbers);
            await db.SaveChangesAsync(ct);
            var newNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);
            foreach (var phone in newNumbers)
            {
                phone.BusinessId = branch.Id;
                branch.PhoneNumbers.Add(phone);
            }
        }

        await db.SaveChangesAsync(ct);
        return new UpdateBranchResponse(branch.Id, branch.Name);
    }
}
