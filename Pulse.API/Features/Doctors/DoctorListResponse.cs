namespace Pulse.API.Features.Doctors;

public record DoctorListResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    string Specialization,
    string Governorate,
    double AverageRating,
    decimal? VisitPrice,
    string? CreatedBy
);
