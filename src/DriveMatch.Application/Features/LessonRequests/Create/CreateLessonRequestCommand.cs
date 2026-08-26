namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed record CreateLessonRequestCommand(
    Guid StudentProfileId,
    Guid InstructorProfileId,
    DateOnly RequestedDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool UsesStudentVehicle,
    string? StudentMessage);
