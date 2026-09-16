using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Abstractions.Persistence.Models;

public sealed record LessonListItem(
    Guid LessonId,
    Guid LessonRequestId,
    string StudentName,
    string InstructorName,
    DateOnly ScheduledDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    LessonStatus Status,
    DateTime? StartedAt,
    DateTime? CheckInAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    DateTime CreatedAt);
