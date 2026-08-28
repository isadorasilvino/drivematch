namespace DriveMatch.Application.Features.LessonRequests.Reject;

public sealed record RejectLessonRequestCommand(
    Guid LessonRequestId,
    Guid UserId);