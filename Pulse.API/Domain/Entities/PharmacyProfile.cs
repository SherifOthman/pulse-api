namespace Pulse.API.Domain.Entities;

/// <summary>
/// Pharmacy-specific profile data. Linked 1:1 to Business via BusinessId (PK = FK).
/// </summary>
public class PharmacyProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public string? LicenseNumber { get; set; }
    public bool Is24Hours { get; set; }
    public bool HasDelivery { get; set; }
    public bool HasInventory { get; set; }
}
