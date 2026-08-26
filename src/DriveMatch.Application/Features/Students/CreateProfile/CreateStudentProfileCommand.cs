using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed record CreateStudentProfileCommand(
    Guid UserId,
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool OwnsVehicle,
    bool HasOwnVehicleForLessons);
