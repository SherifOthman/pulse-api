using Microsoft.AspNetCore.Identity;

namespace Pulse.API.Domain.Entities;

public class ApplicationUser: IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7();
    }

    public string FullName { get; set; } =string.Empty;
    public string? ImageUrl { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = new();

    public List<Testimonial> Testimonials { get; set; }=new();
}


