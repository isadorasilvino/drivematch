using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IStudentProfileRepository
{
    Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StudentProfile studentProfile,
        CancellationToken cancellationToken = default);
}
