namespace DriveMatch.Application.Features.Reviews.Create;

public sealed class LessonNotCompletedException : Exception
{
    public LessonNotCompletedException(Guid lessonId)
        : base($"A aula '{lessonId}' ainda não foi concluída.")
    {
    }
}
