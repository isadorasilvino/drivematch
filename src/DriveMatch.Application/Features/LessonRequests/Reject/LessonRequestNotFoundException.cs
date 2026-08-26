namespace DriveMatch.Application.Features.LessonRequests.Reject;

public sealed class LessonRequestNotFoundException : Exception
{
    public LessonRequestNotFoundException(Guid lessonRequestId)
        : base($"Solicitação de aula '{lessonRequestId}' não encontrada.")
    {
    }
}
