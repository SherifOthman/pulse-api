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
            .CountAsync(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null, ct);

        var pharmaciesCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Pharmacy && b.ParentBusinessId == null, ct);

        var labsCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Laboratory && b.ParentBusinessId == null, ct);

        var radiologyCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Radiology && b.ParentBusinessId == null, ct);

        var branchesCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Doctor && b.ParentBusinessId != null, ct);

        var specializationsCount = await db.Set<Domain.Entities.Specialization>()
            .CountAsync(ct);

        var citiesCount = await db.Set<Domain.Entities.City>()
            .CountAsync(ct);

        // ── Top 5 rated doctors ───────────────────────────────────────────────
        var topDoctors = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .Where(b => b.Testimonials.Any())
            .Select(b => new
            {
                b.Id, b.Name, b.ProfileImageUrl,
                Specialization = b.Doctor!.Specialization.Name,
                Governorate    = b.City.Governorate.Name,
                AvgRating      = b.Testimonials.Average(t => (double)t.Rating),
                TotalRatings   = b.Testimonials.Count,
                b.Doctor.VisitPrice,
            })
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.TotalRatings)
            .Take(5)
            .ToListAsync(ct);

        // ── 6 most recently added doctors ─────────────────────────────────────
        var recentDoctors = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .OrderByDescending(b => b.Id)   // UUIDv7 — creation order
            .Take(6)
            .Select(b => new
            {
                b.Id, b.Name, b.ProfileImageUrl,
                Specialization = b.Doctor!.Specialization.Name,
                Governorate    = b.City.Governorate.Name,
                b.Doctor.VisitPrice,
                b.Doctor.Gender,
            })
            .ToListAsync(ct);

        // ── Specialization distribution (top 8) ───────────────────────────────
        var specializationStats = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .GroupBy(b => b.Doctor!.Specialization.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        // ── Governorate distribution (top 8) ──────────────────────────────────
        var governorateStats = await db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .GroupBy(b => b.City.Governorate.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        return new DashboardResponse(
            doctorsCount,
            pharmaciesCount,
            labsCount,
            radiologyCount,
            branchesCount,
            specializationsCount,
            citiesCount,

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
