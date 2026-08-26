namespace DriveMatch.Application.Features.Instructors.Search;

public sealed record SearchInstructorResult(
    Guid InstructorProfileId,
    Guid UserId,
    string Description,
    int ExperienceYears,
    string City,
    string State,
    decimal PricePerLesson,
    string Currency,
    bool AcceptsBeginners,
    bool AcceptsExperiencedStudents,
    bool AcceptsStudentVehicle);
