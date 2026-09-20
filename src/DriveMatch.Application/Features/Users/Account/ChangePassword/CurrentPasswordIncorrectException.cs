namespace DriveMatch.Application.Features.Users.Account.ChangePassword;

public sealed class CurrentPasswordIncorrectException : Exception
{
    public CurrentPasswordIncorrectException()
        : base("A senha atual está incorreta.")
    {
    }
}