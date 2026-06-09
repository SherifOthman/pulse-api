namespace Pulse.API.Domain.Entities;

public class WorkingDay: Entity
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    public DayOfWeek Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
