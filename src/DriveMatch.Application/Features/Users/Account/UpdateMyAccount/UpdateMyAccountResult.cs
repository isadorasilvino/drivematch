namespace DriveMatch.Application.Features.Users.Account.UpdateMyAccount;

public sealed record UpdateMyAccountResult(
    Guid UserId,
    string Name,
    string Email);