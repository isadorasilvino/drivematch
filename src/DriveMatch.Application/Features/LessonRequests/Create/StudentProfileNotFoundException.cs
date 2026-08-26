namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class StudentProfileNotFoundException : Exception
{
    public StudentProfileNotFoundException(Guid studentProfileId)
        : base($"Perfil de aluno '{studentProfileId}' não encontrado.")
    {
    }
}
