namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed class LessonNotFoundException : Exception
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Aula '{lessonId}' não encontrada.")
    {
    }
}
