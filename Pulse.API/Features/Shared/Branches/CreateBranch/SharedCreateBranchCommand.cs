using MediatR;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Shared.Branches.CreateBranch;

public record SharedCreateBranchCommand(
    Guid BusinessId,
    BusinessType Type,
    string Name,
    Guid? CityId,
    string? Address,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<CreateBranchResponse>;
