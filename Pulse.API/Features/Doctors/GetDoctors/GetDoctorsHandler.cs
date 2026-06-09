using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Doctors.GetDoctors;

public class GetDoctorsHandler(AppDbContext db)
    : IRequestHandler<GetDoctorsQuery, PaginatedResponse<DoctorListResponse>>
{
    public async Task<PaginatedResponse<DoctorListResponse>> Handle(GetDoctorsQuery request, CancellationToken ct)
    {
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        if (request.Gender.HasValue)
            query = query.Where(b => b.Doctor!.Gender == request.Gender.Value);

        if (request.SpecializationId.HasValue)
            query = query.Where(b => b.Doctor!.SpecializationId == request.SpecializationId.Value);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            b.ProfileImageUrl,
            SpecializationName = b.Doctor!.Specialization.Name,
            GovernorateName = b.City.Governorate.Name,
            AvgRating = b.Testimonials
                .Select(t => (double)t.Rating)
                .DefaultIfEmpty()
                .Average(),
            b.Doctor.VisitPrice,
            CreatedBy = b.CreatedByUser != null ? b.CreatedByUser.FullName : null,
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "price" => desc
                ? projected.OrderByDescending(x => x.VisitPrice).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.VisitPrice).ThenBy(x => x.Id),
            "rating" => desc
                ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _ => desc
                ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        var items = raw.Items.Select(r => new DoctorListResponse(
            r.Id,
            r.Name,
            r.ProfileImageUrl,
            r.SpecializationName,
            r.GovernorateName,
            Math.Round(r.AvgRating, 1),
            r.VisitPrice,
            r.CreatedBy
        )).ToList();

        return new PaginatedResponse<DoctorListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
