namespace DriveMatch.Application.Features.Availabilities.ChangeStatus;

public sealed record ChangeAvailabilityStatusCommand(
    Guid AvailabilityId,
    bool IsActive);
