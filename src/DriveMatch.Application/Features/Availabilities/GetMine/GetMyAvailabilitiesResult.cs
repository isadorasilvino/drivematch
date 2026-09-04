namespace DriveMatch.Application.Features.Availabilities.GetMine;

public sealed record GetMyAvailabilitiesResult(
    Guid AvailabilityId,
    Guid InstructorProfileId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);