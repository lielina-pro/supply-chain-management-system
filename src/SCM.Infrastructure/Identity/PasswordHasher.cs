using SCM.Application.Common.Interfaces;

namespace SCM.Infrastructure.Identity;

/// <summary>BCrypt-based password hashing (NFR-7.1: password hashing and encryption for stored credentials).</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
