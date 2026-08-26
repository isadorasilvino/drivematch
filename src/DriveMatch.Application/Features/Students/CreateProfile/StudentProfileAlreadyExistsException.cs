namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed class StudentProfileAlreadyExistsException : Exception
{
    public StudentProfileAlreadyExistsException(Guid userId)
        : base($"O usuário '{userId}' já possui um perfil de aluno.")
    {
    }
}
