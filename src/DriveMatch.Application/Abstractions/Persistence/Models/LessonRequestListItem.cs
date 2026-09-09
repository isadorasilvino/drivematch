using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Abstractions.Persistence.Models;

public sealed record LessonRequestListItem(
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