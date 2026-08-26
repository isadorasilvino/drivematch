namespace DriveMatch.Application.Features.Users.Register;

public sealed record RegisterUserResult(
    Guid UserId,
    string Name,
    string Email);
