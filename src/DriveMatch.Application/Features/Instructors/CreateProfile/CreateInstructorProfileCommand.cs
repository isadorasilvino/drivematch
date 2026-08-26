namespace DriveMatch.Application.Features.Instructors.CreateProfile;

public sealed record CreateInstructorProfileCommand(
    Guid UserId,
    string Description,
    int ExperienceYears,
    string City,
    string State,
    decimal PricePerLesson,
    bool AcceptsBeginners,
    bool AcceptsExperiencedStudents,
    bool AcceptsStudentVehicle);
