using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Instructors.Search;

public sealed record SearchInstructorsQuery(
    string City,
    string State,
    ExperienceLevel ExperienceLevel,
    bool UsesStudentVehicle,
    decimal? MaxPricePerLesson);
