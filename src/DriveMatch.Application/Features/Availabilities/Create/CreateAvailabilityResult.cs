namespace DriveMatch.Application.Features.Availabilities.Create;

public sealed record CreateAvailabilityResult(
    Guid AvailabilityId,
    Guid InstructorProfileId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int LessonDurationMinutes,
    int BreakDurationMinutes);