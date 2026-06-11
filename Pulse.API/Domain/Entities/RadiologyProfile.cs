namespace Pulse.API.Domain.Entities;

/// <summary>
/// Radiology-specific profile data. Linked 1:1 to Business via BusinessId (PK = FK).
/// </summary>
public class RadiologyProfile
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public string? AccreditationNumber { get; set; }

    /// <summary>
    /// Comma-separated list of available imaging modalities
    /// (e.g. "MRI,CT,X-Ray,Ultrasound"). Stored as a plain string.
    /// Parse at the application layer when needed.
    /// </summary>
    public string? AvailableModalities { get; set; }

    public string? EquipmentDetails { get; set; }
}
