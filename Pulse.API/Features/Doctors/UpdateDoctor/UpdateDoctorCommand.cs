using MediatR;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.UpdateDoctor;

public record UpdateDoctorCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    List<Guid>? SpecializationIds,
    Gender? Gender,
    decimal? VisitPrice,
    bool ClearVisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<UpdateDoctorResponse>;
