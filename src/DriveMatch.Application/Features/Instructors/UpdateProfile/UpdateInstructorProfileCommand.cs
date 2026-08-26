namespace DriveMatch.Application.Features.Instructors.UpdateProfile;

public sealed record UpdateInstructorProfileCommand(
    Guid UserId,
    string Description,
    int ExperienceYears,
    string City,
    string State,
    decimal PricePerLesson,
    bool AcceptsBeginners,
    bool AcceptsExperiencedStudents,
    bool AcceptsStudentVehicle);
