using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Testimonials.GetReviews;

public record GetReviewsQuery(
    Guid BusinessId,
    int Page,
    int PageSize
) : IRequest<PaginatedResponse<TestimonialDto>>;
