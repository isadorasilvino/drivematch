using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests.Accept;

public sealed record AcceptLessonRequestResult(
    Guid LessonRequestId,
    LessonRequestStatus LessonRequestStatus,
    Guid LessonId,
    LessonStatus LessonStatus,
    Guid StudentId,
    Guid InstructorId,
    DateOnly ScheduledDate,
    TimeOnly StartTime,
    TimeOnly EndTime);
