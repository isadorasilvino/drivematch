using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Lessons.ConfirmCheckIn;

public sealed record ConfirmLessonCheckInResult(
    Guid LessonId,
    LessonStatus Status,
    DateTime? CheckInAt,
    DateTime? StartedAt);
