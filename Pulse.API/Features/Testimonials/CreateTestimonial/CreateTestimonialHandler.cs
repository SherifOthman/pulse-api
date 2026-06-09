using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Testimonials.CreateTestimonial;

public class CreateTestimonialHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateTestimonialCommand, TestimonialDto>
{
    public async Task<TestimonialDto> Handle(CreateTestimonialCommand request, CancellationToken ct)
    {
        var businessExists = await db.Businesses.AnyAsync(b => b.Id == request.BusinessId, ct);
        if (!businessExists)
            throw new KeyNotFoundException($"Business with id {request.BusinessId} not found");

        var existing = await db.Testimonials
            .AnyAsync(t => t.BusinessId == request.BusinessId && t.UserId == currentUser.Id, ct);
        if (existing)
            throw new InvalidOperationException("You have already reviewed this business");

        var testimonial = new Testimonial
        {
            BusinessId = request.BusinessId,
            UserId = currentUser.Id,
            Rating = Math.Clamp(request.Rating, (byte)1, (byte)5),
            Text = request.Text,
            CreatedAt = DateTimeOffset.Now
        };

        db.Testimonials.Add(testimonial);
        await db.SaveChangesAsync(ct);

        var user = await db.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == currentUser.Id, ct);

        return new TestimonialDto(
            testimonial.Id,
            user.FullName,
            user.ImageUrl,
            testimonial.Rating,
            testimonial.Text,
            testimonial.CreatedAt
        );
    }
}
