using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Lessons.MarkAsNotAttended;

public sealed record MarkLessonAsNotAttendedResult(
    Guid LessonId,
    LessonStatus Status);
