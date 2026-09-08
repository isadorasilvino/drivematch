namespace DriveMatch.Application.Features.Availabilities.GetAvailableSlots;

public sealed record AvailableSlotResult(
    TimeOnly StartTime,
    TimeOnly EndTime);