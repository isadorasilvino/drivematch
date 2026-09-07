namespace DriveMatch.Application.Abstractions.Persistence;

public sealed record InstructorSearchItem(
    Guid InstructorProfileId,
    Guid UserId,
    string Name,
    string Description,
    int ExperienceYears,
    string City,
    string State,
    decimal PricePerLesson,
    string Currency,
    bool AcceptsBeginners,
    bool AcceptsExperiencedStudents,
    bool AcceptsStudentVehicle);