namespace DriveMatch.Application.Features.Users.Account.UpdateMyAccount;

public sealed class AccountEmailAlreadyInUseException : Exception
{
    public AccountEmailAlreadyInUseException(string email)
        : base($"O e-mail '{email}' já está em uso.")
    {
    }
}