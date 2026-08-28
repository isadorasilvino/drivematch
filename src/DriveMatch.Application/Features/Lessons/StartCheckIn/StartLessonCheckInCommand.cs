namespace DriveMatch.Application.Features.Lessons.StartCheckIn;

public sealed record StartLessonCheckInCommand(
    Guid LessonId,
    Guid UserId);