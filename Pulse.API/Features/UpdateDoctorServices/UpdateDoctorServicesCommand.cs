using MediatR;

namespace Pulse.API.Features.Doctors.Services.UpdateDoctorServices;

/// <summary>
/// Replaces the full set of services for a doctor.
/// Pass an empty list to remove all services.
/// Each item is either an existing ServiceId or a new service name to create.
/// </summary>
public record UpdateDoctorServicesCommand(
    Guid DoctorId,
    List<ServiceItem> Services
) : IRequest<UpdateDoctorServicesResponse>;

/// <param name="Id">Existing service ID — null when creating a new service.</param>
/// <param name="Name">New service name — only used when Id is null.</param>
public record ServiceItem(Guid? Id, string? Name);
