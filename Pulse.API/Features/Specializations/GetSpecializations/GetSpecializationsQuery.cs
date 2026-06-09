using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public record GetSpecializationsQuery : IRequest<List<SpecializationResponse>>;
