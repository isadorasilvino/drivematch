namespace DriveMatch.Application.Features.Instructors.ChangeStatus;

public sealed record ChangeInstructorProfileStatusCommand(
    Guid UserId,
    bool IsActive);
