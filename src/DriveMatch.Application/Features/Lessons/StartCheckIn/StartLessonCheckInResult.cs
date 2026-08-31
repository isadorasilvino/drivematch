using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Lessons.StartCheckIn;

public sealed record StartLessonCheckInResult(
    Guid LessonId,
    LessonStatus Status,
    string CheckInToken,
    DateTime CheckInTokenExpiresAt);