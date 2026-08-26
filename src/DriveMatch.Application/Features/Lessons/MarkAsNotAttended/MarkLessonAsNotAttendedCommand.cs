namespace DriveMatch.Application.Features.Lessons.MarkAsNotAttended;

public sealed record MarkLessonAsNotAttendedCommand(
    Guid LessonId);
