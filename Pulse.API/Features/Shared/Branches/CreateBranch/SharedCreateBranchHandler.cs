using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.CreateBranch;

public class SharedCreateBranchHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<SharedCreateBranchCommand, CreateBranchResponse>
{
    public async Task<CreateBranchResponse> Handle(SharedCreateBranchCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Branch name is required");

        var parent = await db.Businesses
            .FirstOrDefaultAsync(b =>
                b.Id == request.BusinessId &&
                b.Type == request.Type &&
                b.ParentBusinessId == null, ct);

        if (parent is null)
            throw new NotFoundException("Parent business not found");

        var branch = new Business
        {
            Name             = request.Name.Trim(),
            Type             = request.Type,
            CityId           = request.CityId ?? parent.CityId,
            Address          = request.Address?.Trim(),
            Latitude         = request.Latitude,
            Longitude        = request.Longitude,
            ParentBusinessId = request.BusinessId,
            CreatedByUserId  = currentUser.Id,
            WorkingDays  = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays),
            PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers),
        };

        db.Businesses.Add(branch);
        await db.SaveChangesAsync(ct);

        return new CreateBranchResponse(branch.Id, branch.Name);
    }
}
