using MediatR;

namespace Pulse.API.Features.Dashboard;

public record DashboardQuery() : IRequest<DashboardResponse>;
