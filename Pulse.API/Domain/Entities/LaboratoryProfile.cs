namespace Pulse.API.Domain.Entities;

/// <summary>
/// Laboratory-specific profile data. Linked 1:1 to Business via BusinessId (PK = FK).
/// </summary>
public class LaboratoryProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public string? AccreditationNumber { get; set; }
    public bool OffersHomeVisit { get; set; }
    public string? TestCatalogUrl { get; set; }
}
