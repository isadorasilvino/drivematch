namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId)
        : base($"Usuário '{userId}' não encontrado.")
    {
    }
}
