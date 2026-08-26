using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed record CancelLessonResult(
    Guid LessonId,
    LessonStatus Status,
    DateTime? CancelledAt);
