using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests.Reject;

public sealed record RejectLessonRequestResult(
    Guid LessonRequestId,
    LessonRequestStatus Status);
