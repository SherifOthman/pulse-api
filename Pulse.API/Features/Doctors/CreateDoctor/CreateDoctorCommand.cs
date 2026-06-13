using MediatR;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.CreateDoctor;

public record CreateDoctorCommand(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    Guid? SpecializationId,
    Gender? Gender,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<CreateDoctorResponse>;
