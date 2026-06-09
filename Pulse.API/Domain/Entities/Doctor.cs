using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

public class Doctor
{
    public Guid BusinessId { get;set;  }
    public Business Business { get; set; } = null!;
    public Guid SpecializationId { get;set; }
    public Specialization Specialization { get; set; } = null!;
    public decimal? VisitPrice { get; set; }
    public Gender Gender { get; set; }
}
