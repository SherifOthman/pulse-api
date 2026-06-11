using Pulse.API.Domain.Enums;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Registers generic business-services endpoints for Pharmacy, Lab, and Radiology.
/// Doctor services use dedicated feature slices.
/// </summary>
public class BusinessServicesRegistration : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        BusinessServicesEndpoints.MapForBusinessType(app, "pharmacies", BusinessType.Pharmacy);
        BusinessServicesEndpoints.MapForBusinessType(app, "labs",       BusinessType.Laboratory);
        BusinessServicesEndpoints.MapForBusinessType(app, "radiology",  BusinessType.Radiology);
    }
}
