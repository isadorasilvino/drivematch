using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Instructors.CreateProfile;

public sealed record CreateInstructorProfileResult(
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
    bool AcceptsStudentVehicle,
    InstructorProfileStatus Status);
