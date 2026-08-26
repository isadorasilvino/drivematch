using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed record CreateLessonRequestResult(
    Guid LessonRequestId,
    Guid StudentId,
    Guid InstructorId,
    DateOnly RequestedDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool UsesStudentVehicle,
    string? StudentMessage,
    LessonRequestStatus Status);
