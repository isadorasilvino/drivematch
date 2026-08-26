namespace DriveMatch.Application.Features.LessonRequests.Accept;

public sealed class LessonRequestNotFoundException : Exception
{
    public LessonRequestNotFoundException(Guid lessonRequestId)
        : base($"Solicitação de aula '{lessonRequestId}' não encontrada.")
    {
    }
}
