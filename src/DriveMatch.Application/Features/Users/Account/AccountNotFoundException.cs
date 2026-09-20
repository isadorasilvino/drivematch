namespace DriveMatch.Application.Features.Users.Account;

public sealed class AccountNotFoundException : Exception
{
    public AccountNotFoundException(Guid userId)
        : base($"Conta do usuário '{userId}' não encontrada.")
    {
    }
}