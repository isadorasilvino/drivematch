namespace DriveMatch.Application.Features.Lessons.MarkAsNotAttended;

public sealed class LessonNotFoundException : Exception
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Aula '{lessonId}' não encontrada.")
    {
    }
}
