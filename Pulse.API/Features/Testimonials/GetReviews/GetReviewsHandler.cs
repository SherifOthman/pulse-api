using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Testimonials.GetReviews;

public class GetReviewsHandler(AppDbContext db) : IRequestHandler<GetReviewsQuery, PaginatedResponse<TestimonialDto>>
{
    public async Task<PaginatedResponse<TestimonialDto>> Handle(GetReviewsQuery request, CancellationToken ct)
    {
        var query = db.Set<Domain.Entities.Testimonial>()
            .AsNoTracking()
            .Where(t => t.BusinessId == request.BusinessId)
            .OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new
            {
                t.Id,
                UserName    = t.User.FullName,
                UserImageUrl = t.User.ImageUrl,
                t.Rating,
                t.Text,
                t.CreatedAt,
            })
            .ToListAsync(ct);

        var dtos = items
            .Select(t => new TestimonialDto(t.Id, t.UserName, t.UserImageUrl, t.Rating, t.Text, t.CreatedAt))
            .ToList();

        return new PaginatedResponse<TestimonialDto>(
            dtos, request.Page, request.PageSize, total, request.Page * request.PageSize < total);
    }
}
