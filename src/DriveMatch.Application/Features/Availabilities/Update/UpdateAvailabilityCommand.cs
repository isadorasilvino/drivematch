namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed record UpdateAvailabilityCommand(
    Guid AvailabilityId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime);
