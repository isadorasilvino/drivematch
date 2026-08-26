using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Users.Register;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role);
