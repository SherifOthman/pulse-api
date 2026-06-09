namespace Pulse.API.Options;

public class GoogleAuthOptions
{
    public string WebClientId { get; set; } = null!;

    public string WebClientSecret { get; set; } = null!;

    public string? AndroidClientId { get; set; }

    public string? IosClientId { get; set; }
}
