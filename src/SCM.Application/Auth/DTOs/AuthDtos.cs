namespace SCM.Application.Auth.DTOs;

// Placeholder DTOs so AuthController compiles. Full implementation is
// Bethel's Week 3 task (JWT auth: login, token issue, password hash).
public class LoginRequest        { public string Email { get; set; } = default!; public string Password { get; set; } = default!; }
public class RegisterUserRequest { public string FullName { get; set; } = default!; public string Email { get; set; } = default!; public string Password { get; set; } = default!; public string Role { get; set; } = default!; }
public class AuthResponse        { public string Token { get; set; } = default!; public DateTime ExpiresAt { get; set; } }
