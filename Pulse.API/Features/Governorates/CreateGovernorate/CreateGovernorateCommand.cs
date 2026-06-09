using MediatR;

namespace Pulse.API.Features.Governorates.CreateGovernorate;

public record CreateGovernorateCommand(string Name) : IRequest<GovernorateResponse>;
