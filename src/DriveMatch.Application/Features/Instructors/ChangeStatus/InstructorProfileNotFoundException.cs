namespace DriveMatch.Application.Features.Instructors.ChangeStatus;

public sealed class InstructorProfileNotFoundException : Exception
{
    public InstructorProfileNotFoundException(Guid userId)
        : base($"Perfil de instrutor do usuário '{userId}' não encontrado.")
    {
    }
}
