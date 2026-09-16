namespace DriveMatch.Application.Features.LessonRequests.Cancel;

public sealed record CancelLessonRequestCommand(
    Guid LessonRequestId,
    Guid UserId);