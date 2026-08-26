namespace DriveMatch.Application.Features.Users.Register;

public sealed class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"Já existe um usuário cadastrado com o email '{email}'.")
    {
    }
}
