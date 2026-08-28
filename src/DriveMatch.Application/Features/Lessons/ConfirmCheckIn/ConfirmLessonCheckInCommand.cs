namespace DriveMatch.Application.Features.Lessons.ConfirmCheckIn;

public sealed record ConfirmLessonCheckInCommand(
    Guid LessonId,
    Guid UserId);