using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Laboratories.GetLaboratories;

public class GetLaboratoriesHandler(AppDbContext db)
    : IRequestHandler<GetLaboratoriesQuery, PaginatedResponse<LaboratoryListResponse>>
{
    public async Task<PaginatedResponse<LaboratoryListResponse>> Handle(GetLaboratoriesQuery request, CancellationToken ct)
    {
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Laboratory && b.ParentBusinessId == null)
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

        var items = raw.Items.Select(r => new LaboratoryListResponse(
            r.Id,
            r.Name,
            r.GovernorateName,
            Math.Round(r.AvgRating, 1),
            r.CreatedBy
        )).ToList();

        return new PaginatedResponse<LaboratoryListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
