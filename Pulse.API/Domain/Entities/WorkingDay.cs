namespace Pulse.API.Domain.Entities;

public class WorkingDay : Entity
{
    // Exactly one of these two will be set — the other will be null.
    public Guid? BusinessId { get; set; }
    public Business? Business { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public DayOfWeek Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
