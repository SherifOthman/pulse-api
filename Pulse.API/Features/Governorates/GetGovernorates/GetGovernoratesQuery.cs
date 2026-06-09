using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorates;

public record GetGovernoratesQuery(int? BusinessType) : IRequest<List<GovernorateResponse>>;
