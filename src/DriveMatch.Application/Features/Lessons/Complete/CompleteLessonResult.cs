using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Lessons.Complete;

public sealed record CompleteLessonResult(
    Guid LessonId,
    LessonStatus Status,
    DateTime? CompletedAt);
