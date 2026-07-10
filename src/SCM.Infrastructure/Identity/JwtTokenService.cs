using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SCM.Application.Common.Interfaces;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Identity;

/// <summary>
/// Builds and signs JWT access tokens.
/// Reads Jwt:Key, Jwt:Issuer, Jwt:Audience, Jwt:ExpiryMinutes from configuration.
/// FR-01.1, FR-01.3
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user, IEnumerable<string> roleNames)
    {
        var key     = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
                          double.Parse(_config["Jwt:ExpiryMinutes"] ?? "480"));

        // Build claims — one ClaimTypes.Role per role so RBAC policies work
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("fullName",                    user.FullName),
        };

        foreach (var role in roleNames)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}