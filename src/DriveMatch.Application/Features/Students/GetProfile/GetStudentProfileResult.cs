using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.GetProfile;

public sealed record GetStudentProfileResult(
    Guid StudentProfileId,
    Guid UserId,
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool OwnsVehicle,
    bool HasOwnVehicleForLessons);