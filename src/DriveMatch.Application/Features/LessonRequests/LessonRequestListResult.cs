using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests;

public sealed record LessonRequestListResult(
    Guid LessonRequestId,
    string StudentName,
    string InstructorName,
    DateOnly RequestedDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool UsesStudentVehicle,
    string? StudentMessage,
    LessonRequestStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);