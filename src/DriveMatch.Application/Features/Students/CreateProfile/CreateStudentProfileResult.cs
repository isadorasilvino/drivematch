using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed record CreateStudentProfileResult(
    Guid StudentProfileId,
    Guid UserId,
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool OwnsVehicle,
    bool HasOwnVehicleForLessons);
