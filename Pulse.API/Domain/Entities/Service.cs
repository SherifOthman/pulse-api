using Pulse.API.Domain.Enums;

namespace Pulse.API.Domain.Entities;

public class Service : Entity
{
    public BusinessType BusinessType { get; set; }
    public string Name { get; set; } = null!;

    public List<BusinessService> BusinessServices { get; set; } = new();

}
