namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class InstructorProfileNotFoundException : Exception
{
    public InstructorProfileNotFoundException(Guid instructorProfileId)
        : base($"Perfil de instrutor '{instructorProfileId}' não encontrado.")
    {
    }
}
