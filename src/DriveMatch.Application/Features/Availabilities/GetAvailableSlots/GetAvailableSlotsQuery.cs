namespace DriveMatch.Application.Features.Availabilities.GetAvailableSlots;

public sealed record GetAvailableSlotsQuery(
    Guid InstructorProfileId,
    DateOnly Date);