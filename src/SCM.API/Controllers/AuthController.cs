using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Auth.DTOs;
using SCM.Application.Auth.Interfaces;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>FR-01.1 – Login, receive JWT token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _auth.LoginAsync(req);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }

    /// <summary>FR-01.2 – Register user with role (Admin only)</summary>
    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest req)
    {
        var result = await _auth.RegisterAsync(req);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
