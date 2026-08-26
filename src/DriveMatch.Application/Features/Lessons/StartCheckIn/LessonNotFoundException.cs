namespace DriveMatch.Application.Features.Lessons.StartCheckIn;

public sealed class LessonNotFoundException : Exception
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Aula '{lessonId}' não encontrada.")
    {
    }
}
