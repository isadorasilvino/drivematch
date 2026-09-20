using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Users.Account.GetMyAccount;

public sealed record GetMyAccountResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role);