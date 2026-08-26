using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IInstructorProfileRepository
{
    Task<InstructorProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InstructorProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
        string city,
        string state,
        ExperienceLevel experienceLevel,
        bool usesStudentVehicle,
        decimal? maxPricePerLesson,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InstructorProfile instructorProfile,
        CancellationToken cancellationToken = default);
}
