namespace DriveMatch.Application.Features.Auth.Login;

public sealed class UserInactiveException : Exception
{
    public UserInactiveException()
        : base("O usuário está inativo.")
    {
    }
}
