using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.UpdateProfile;

public sealed record UpdateStudentProfileCommand(
    Guid UserId,
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool OwnsVehicle,
    bool HasOwnVehicleForLessons);
