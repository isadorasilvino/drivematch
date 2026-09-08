namespace DriveMatch.Domain.ValueObjects;

public sealed record AvailabilitySlot(
    TimeOnly StartTime,
    TimeOnly EndTime);