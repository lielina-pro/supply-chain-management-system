using SCM.Domain.Entities;

namespace SCM.Application.Common.Interfaces;

/// <summary>
/// Generates a signed JWT access token for an authenticated user.
/// FR-01.1, FR-01.3 — token carries role claims used by RBAC policies.
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user, IEnumerable<string> roleNames);
}
