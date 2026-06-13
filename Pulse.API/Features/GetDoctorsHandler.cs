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
            .Where(b => b.Type == BusinessType.Doctor)
            .ApplyFilters(bq);

        if (request.Gender.HasValue)
            query = query.Where(b => b.DoctorProfile!.Gender == request.Gender.Value);

        if (request.SpecializationId.HasValue)
            query = query.Where(b => b.DoctorProfile!.DoctorSpecializations
                .Any(ds => ds.SpecializationId == request.SpecializationId.Value));

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            b.ProfileImageUrl,
            GovernorateName = b.City.Governorate.Name,
            AvgRating       = b.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
            b.DoctorProfile!.Gender,
            b.DoctorProfile!.VisitPrice,
            CreatedBy = b.CreatedByUser != null ? b.CreatedByUser.FullName : null,
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "rating" => desc ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                              : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _        => desc ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                              : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        // Load specializations for this page of doctors in one query
        var doctorIds = raw.Items.Select(r => r.Id).ToList();
        var specializationsByDoctor = await db.DoctorSpecializations
            .AsNoTracking()
            .Where(ds => doctorIds.Contains(ds.DoctorProfileId))
            .Select(ds => new { ds.DoctorProfileId, ds.Specialization.Name })
            .ToListAsync(ct);

        var specMap = specializationsByDoctor
            .GroupBy(x => x.DoctorProfileId)
            .ToDictionary(g => g.Key, g => string.Join("، ", g.Select(x => x.Name)));

        var items = raw.Items.Select(r => new DoctorListResponse(
            r.Id, r.Name, r.ProfileImageUrl,
            specMap.TryGetValue(r.Id, out var spec) ? spec : "",
            r.GovernorateName,
            Math.Round(r.AvgRating, 1),
            (int)r.Gender, r.CreatedBy, r.VisitPrice
        )).ToList();

        return new PaginatedResponse<DoctorListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
