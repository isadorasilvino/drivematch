namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class InstructorNotActiveException : Exception
{
    public InstructorNotActiveException(Guid instructorProfileId)
        : base($"O perfil de instrutor '{instructorProfileId}' não está ativo.")
    {
    }
}
