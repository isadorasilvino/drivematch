namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed record UpdateAvailabilityResult(
    Guid AvailabilityId,
    Guid InstructorProfileId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int LessonDurationMinutes,
    int BreakDurationMinutes,
    bool IsActive);