namespace Pulse.API.Features.Dashboard;

public record DashboardResponse(
    int DoctorsCount,
    int PharmaciesCount,
    int LabsCount,
    int RadiologyCount
);
