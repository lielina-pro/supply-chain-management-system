using SCM.Application.Auth.DTOs;
using SCM.Application.Auth.Interfaces;
using SCM.Application.Common.Interfaces;
using SCM.Application.Common.Models;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces;

namespace SCM.Application.Auth.Services;

/// <summary>
/// Implements login and registration workflows.
/// FR-01.1 (login + JWT), FR-01.2 (roles), FR-01.3 (RBAC via role claims).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork      _uow;
    private readonly IPasswordHasher  _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthService(
        IUnitOfWork      uow,
        IPasswordHasher  hasher,
        IJwtTokenService jwt)
    {
        _uow    = uow;
        _hasher = hasher;
        _jwt    = jwt;
    }

    // ── FR-01.1 Login ────────────────────────────────────────────────────────
    public async Task<ServiceResult<AuthResponse>> LoginAsync(
        LoginRequest req, CancellationToken ct = default)
    {
        // 1. Look up user by email
        var user = await _uow.Users.GetByEmailAsync(req.Email, ct);

        // 2. Verify password — generic message so we don't leak which was wrong
        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash))
            return ServiceResult<AuthResponse>.Failure("Invalid email or password.");

        // 3. Check account is active
        if (!user.IsActive)
            return ServiceResult<AuthResponse>.Failure("Account is inactive.");

        // 4. Load role names from UserRoles navigation
        var roleNames = user.UserRoles
            .Where(ur => !ur.IsArchived)
            .Select(ur => ur.Role.Name)
            .ToList();

        // 5. Generate JWT
        var token     = _jwt.GenerateToken(user, roleNames);
        var expiresAt = DateTime.UtcNow.AddMinutes(480);

        // 6. Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        // 7. Write audit log (BR-10)
        await _uow.AuditLogs.AddAsync(new AuditLog
        {
            UserId     = user.Id,
            Action     = "Login",
            EntityName = "User",
            EntityId   = user.Id,
            Details    = "Successful login",
            Timestamp  = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return ServiceResult<AuthResponse>.Success(new AuthResponse
        {
            Token     = token,
            ExpiresAt = expiresAt
        });
    }

    // ── FR-01.2 Register ─────────────────────────────────────────────────────
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(
        RegisterUserRequest req, CancellationToken ct = default)
    {
        // 1. Check email uniqueness
        if (await _uow.Users.EmailExistsAsync(req.Email, ct))
            return ServiceResult<AuthResponse>.Failure(
                "A user with this email already exists.");

        // 2. Validate role exists
        var role = await _uow.Roles.GetByNameAsync(req.Role, ct);
        if (role is null)
            return ServiceResult<AuthResponse>.Failure(
                $"Role '{req.Role}' does not exist.");

        var now = DateTime.UtcNow;

        // 3. Create user with hashed password
        var user = new User
        {
            FullName     = req.FullName,
            Email        = req.Email,
            PasswordHash = _hasher.Hash(req.Password),
            IsActive     = true,
            CreatedAt    = now
        };

        // 4. Assign role (BR-09: every user must have at least one role)
        user.UserRoles.Add(new UserRole
        {
            User       = user,
            Role       = role,
            AssignedAt = now
        });

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // 5. Write audit log (BR-10)
        await _uow.AuditLogs.AddAsync(new AuditLog
        {
            UserId     = user.Id,
            Action     = "Register",
            EntityName = "User",
            EntityId   = user.Id,
            Details    = $"User registered with role '{req.Role}'",
            Timestamp  = now
        }, ct);
        await _uow.SaveChangesAsync(ct);

        // 6. Generate token and return
        var token = _jwt.GenerateToken(user, new[] { role.Name });

        return ServiceResult<AuthResponse>.Success(new AuthResponse
        {
            Token     = token,
            ExpiresAt = now.AddMinutes(480)
        });
    }
}