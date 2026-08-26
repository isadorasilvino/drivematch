using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.UpdateProfile;

public sealed record UpdateStudentProfileResult(
    Guid StudentProfileId,
    Guid UserId,
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool OwnsVehicle,
    bool HasOwnVehicleForLessons,
    DateTime? UpdatedAt);
