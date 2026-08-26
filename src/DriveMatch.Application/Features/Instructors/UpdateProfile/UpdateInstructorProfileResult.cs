using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Instructors.UpdateProfile;

public sealed record UpdateInstructorProfileResult(
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
    InstructorProfileStatus Status,
    DateTime? UpdatedAt);
