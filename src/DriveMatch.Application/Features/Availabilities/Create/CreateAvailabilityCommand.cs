namespace DriveMatch.Application.Features.Availabilities.Create;

public sealed record CreateAvailabilityCommand(
    Guid UserId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int LessonDurationMinutes,
    int BreakDurationMinutes);