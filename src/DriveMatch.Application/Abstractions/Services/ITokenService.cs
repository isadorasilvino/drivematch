using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateToken(
        Guid userId,
        string email,
        UserRole role);
}