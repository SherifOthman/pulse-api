using Pulse.API.Domain.Entities;

namespace Pulse.API.Infrastructure;

public interface IJwtTokenGenerator
{
    string Generate(ApplicationUser user, IList<string> roles);
}
