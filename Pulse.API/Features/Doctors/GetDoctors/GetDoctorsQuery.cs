using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Doctors.GetDoctors;

public record GetDoctorsQuery(
    BusinessQuery BusinessQuery,
    Gender? Gender,
    Guid? SpecializationId
) : IRequest<PaginatedResponse<DoctorListResponse>>;
