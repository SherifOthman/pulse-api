namespace Pulse.API.Features.Doctors;

public record DoctorListResponse(
    Guid Id,
    string Name,
    string Specialization,
    string Governorate,
    double AverageRating,
    decimal? VisitPrice,
    string? CreatedBy
);
