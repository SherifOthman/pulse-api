using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public record GetSpecializationsQuery(int? BusinessType = null) : IRequest<List<SpecializationResponse>>;
