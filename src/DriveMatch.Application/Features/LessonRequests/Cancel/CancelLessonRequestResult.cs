using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests.Cancel;

public sealed record CancelLessonRequestResult(
    Guid LessonRequestId,
    LessonRequestStatus Status);