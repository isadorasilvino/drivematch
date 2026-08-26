using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IStudentProfileRepository
{
    Task<StudentProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StudentProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StudentProfile studentProfile,
        CancellationToken cancellationToken = default);
}
