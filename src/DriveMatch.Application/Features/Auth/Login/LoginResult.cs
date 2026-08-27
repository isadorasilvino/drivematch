using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Auth.Login;

public sealed record LoginResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    string Token);