using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Instructors.ChangeStatus;

public sealed record ChangeInstructorProfileStatusResult(
    Guid InstructorProfileId,
    InstructorProfileStatus Status);
