using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Radiology.GetRadiology;

public class GetRadiologyHandler(AppDbContext db)
    : IRequestHandler<GetRadiologyQuery, PaginatedResponse<RadiologyListResponse>>
{
    public async Task<PaginatedResponse<RadiologyListResponse>> Handle(GetRadiologyQuery request, CancellationToken ct)
    {
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Radiology && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            GovernorateName = b.City.Governorate.Name,
            AvgRating = b.Testimonials
                .Select(t => (double)t.Rating)
                .DefaultIfEmpty()
                .Average(),
            CreatedBy = b.CreatedByUser != null ? b.CreatedByUser.FullName : null,
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "rating" => desc
                ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _ => desc
                ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        var items = raw.Items.Select(r => new RadiologyListResponse(
            r.Id,
            r.Name,
            r.GovernorateName,
            Math.Round(r.AvgRating, 1),
            r.CreatedBy
        )).ToList();

        return new PaginatedResponse<RadiologyListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
