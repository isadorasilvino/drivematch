namespace DriveMatch.Application.Features.Reviews.Create;

public sealed class LessonNotFoundException : Exception
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Aula '{lessonId}' não encontrada.")
    {
    }
}
