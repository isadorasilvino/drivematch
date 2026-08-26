using DriveMatch.Application.Abstractions.Services;

namespace DriveMatch.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
