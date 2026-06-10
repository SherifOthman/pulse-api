namespace Pulse.API.Features.Dashboard;

public record DashboardResponse(
    // ── Counts ────────────────────────────────────────────────────────────────
    int DoctorsCount,
    int PharmaciesCount,
    int LabsCount,
    int RadiologyCount,
    int BranchesCount,
    int SpecializationsCount,
    int CitiesCount,

    // ── Top rated doctors ─────────────────────────────────────────────────────
    List<TopDoctorDto> TopRatedDoctors,

    // ── Recently added doctors ────────────────────────────────────────────────
    List<RecentDoctorDto> RecentDoctors,

    // ── Specialization distribution ───────────────────────────────────────────
    List<SpecializationStatDto> SpecializationStats,

    // ── Governorate distribution ──────────────────────────────────────────────
    List<GovernorateStatDto> GovernorateStats
);

public record TopDoctorDto(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Specialization,
    string Governorate,
    double AverageRating,
    int TotalRatings,
    decimal? VisitPrice
);

public record RecentDoctorDto(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Specialization,
    string Governorate,
    decimal? VisitPrice,
    int Gender
);

public record SpecializationStatDto(
    string Name,
    int Count
);

public record GovernorateStatDto(
    string Name,
    int Count
);
