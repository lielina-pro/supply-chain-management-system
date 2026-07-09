using SCM.Application.Auth.DTOs;
using SCM.Application.Common.Models;

namespace SCM.Application.Auth.Interfaces;

// Placeholder so AuthController compiles. Not implemented/registered yet -
// implementing this and JWT issuance is Bethel's Week 3 task (FR-01.1-01.2).
public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken ct = default);
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterUserRequest req, CancellationToken ct = default);
}
