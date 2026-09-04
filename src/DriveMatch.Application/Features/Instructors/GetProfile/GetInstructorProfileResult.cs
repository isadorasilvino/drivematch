using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Instructors.GetProfile;

public sealed record GetInstructorProfileResult(
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