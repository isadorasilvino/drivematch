namespace DriveMatch.Application.Features.Auth.Login;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Email ou senha inválidos.")
    {
    }
}
