using MediatR;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Testimonials.CreateTestimonial;

public record CreateTestimonialCommand(
    Guid BusinessId,
    byte Rating,
    string Text
) : IRequest<TestimonialDto>;
