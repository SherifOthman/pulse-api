namespace Pulse.API.Features.Doctors;

public record DoctorListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Specialization,       // comma-joined for display
    string Governorate,
    double AverageRating,
    int Gender,        // 0=Male, 1=Female
    string? CreatedBy,
    decimal? VisitPrice
);
