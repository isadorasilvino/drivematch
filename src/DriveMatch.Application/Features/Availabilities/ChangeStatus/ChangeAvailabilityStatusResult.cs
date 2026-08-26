namespace DriveMatch.Application.Features.Availabilities.ChangeStatus;

public sealed record ChangeAvailabilityStatusResult(
    Guid AvailabilityId,
    bool IsActive);
