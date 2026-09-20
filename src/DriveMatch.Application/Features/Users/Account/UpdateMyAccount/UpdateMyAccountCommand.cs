namespace DriveMatch.Application.Features.Users.Account.UpdateMyAccount;

public sealed record UpdateMyAccountCommand(
    Guid UserId,
    string Name,
    string Email);