using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Doctors.GetMobileDoctors;

public record GetMobileDoctorsQuery(
    BusinessQuery BusinessQuery,
    Gender? Gender,
    Guid? SpecializationId
) : IRequest<PaginatedResponse<DoctorMobileListResponse>>;
