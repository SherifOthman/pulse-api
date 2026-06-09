namespace Pulse.API.Options;

public class JWTOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Adudience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; }
    public int RefreshTokenExpireDays { get; set; }
}
