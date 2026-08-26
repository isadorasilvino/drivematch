namespace DriveMatch.Application.Features.Students.UpdateProfile;

public sealed class StudentProfileNotFoundException : Exception
{
    public StudentProfileNotFoundException(Guid userId)
        : base($"Perfil de aluno não encontrado para o usuário '{userId}'.")
    {
    }
}
