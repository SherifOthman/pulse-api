using MediatR;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Radiology.UpdateRadiology;

public record UpdateRadiologyCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays = null,
    List<PhoneNumberDto>? PhoneNumbers = null
) : IRequest<RadiologyResponse>;
