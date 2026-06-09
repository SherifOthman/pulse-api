using System.Security.Claims;

namespace Pulse.API.Infrastructure;

public interface ICurrentUser
{
    Guid Id { get; }
}
