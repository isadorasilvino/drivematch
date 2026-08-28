namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed record UpdateAvailabilityCommand(
    Guid AvailabilityId,
    Guid UserId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime);