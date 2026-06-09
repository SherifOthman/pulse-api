using MediatR;
using Pulse.API.Domain.Enums;

namespace Pulse.API.Features.Doctors.UpdateDoctor;

public record UpdateDoctorCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    Guid? SpecializationId,
    decimal? VisitPrice,
    Gender? Gender
) : IRequest<UpdateDoctorResponse>;
