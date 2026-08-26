namespace DriveMatch.Application.Features.Lessons.Complete;

public sealed class LessonNotFoundException : Exception
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Aula '{lessonId}' não encontrada.")
    {
    }
}
