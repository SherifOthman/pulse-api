using MediatR;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Shared.Branches.UpdateBranch;

public record SharedUpdateBranchCommand(
    Guid Id,
    BusinessType Type,
    string? Name,
    Guid? CityId,
    string? Address,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<UpdateBranchResponse>;
