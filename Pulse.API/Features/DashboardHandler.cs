using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Dashboard;

public class DashboardHandler(AppDbContext db) : IRequestHandler<DashboardQuery, DashboardResponse>
{
    public async Task<DashboardResponse> Handle(DashboardQuery request, CancellationToken ct)
    {
        // ── Counts ────────────────────────────────────────────────────────────
        var doctorsCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Doctor, ct);

        var pharmaciesCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Pharmacy, ct);

        var labsCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Laboratory, ct);

        var radiologyCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Radiology, ct);

        // Branches now live in a separate table
        var branchesCount = await db.Branches.CountAsync(ct);

        var specializationsCount = await db.Set<Domain.Entities.Specialization>()
            .CountAsync(ct);

        var citiesCount = await db.Set<Domain.Entities.City>()
            .CountAsync(ct);

        // ── Top 5 rated doctors ───────────────────────────────────────────────
        var topDoctors = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.Testimonials.Any())
            .Select(b => new
            {
                b.Id, b.Name, b.ProfileImageUrl,
                Specialization = string.Join("، ", b.DoctorProfile!.DoctorSpecializations
                    .Select(ds => ds.Specialization.Name)),
                Governorate    = b.City.Governorate.Name,
                AvgRating      = b.Testimonials.Average(t => (double)t.Rating),
                TotalRatings   = b.Testimonials.Count,
                VisitPrice     = b.DoctorProfile!.VisitPrice,
            })
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.TotalRatings)
            .Take(5)
            .ToListAsync(ct);

        // ── 6 most recently added doctors ─────────────────────────────────────
        var recentDoctors = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor)
            .OrderByDescending(b => b.Id)
            .Take(6)
            .Select(b => new
            {
                b.Id, b.Name, b.ProfileImageUrl,
                Specialization = string.Join("، ", b.DoctorProfile!.DoctorSpecializations
                    .Select(ds => ds.Specialization.Name)),
                Governorate    = b.City.Governorate.Name,
                b.DoctorProfile.Gender,
                VisitPrice     = b.DoctorProfile!.VisitPrice,
            })
            .ToListAsync(ct);

        // ── Specialization distribution (top 8) ───────────────────────────────
        var specializationStats = await db.DoctorSpecializations
            .AsNoTracking()
            .GroupBy(ds => ds.Specialization.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        // ── Governorate distribution (top 8) ──────────────────────────────────
        var governorateStats = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor)
            .GroupBy(b => b.City.Governorate.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        return new DashboardResponse(
            doctorsCount, pharmaciesCount, labsCount, radiologyCount,
            branchesCount, specializationsCount, citiesCount,

            topDoctors.Select(d => new TopDoctorDto(
                d.Id, d.Name, d.ProfileImageUrl,
                d.Specialization, d.Governorate,
                Math.Round(d.AvgRating, 1), d.TotalRatings, d.VisitPrice
            )).ToList(),

            recentDoctors.Select(d => new RecentDoctorDto(
                d.Id, d.Name, d.ProfileImageUrl,
                d.Specialization, d.Governorate,
                d.VisitPrice, (int)d.Gender
            )).ToList(),

            specializationStats.Select(s => new SpecializationStatDto(s.Name, s.Count)).ToList(),
            governorateStats.Select(g => new GovernorateStatDto(g.Name, g.Count)).ToList()
        );
    }
}
