using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IInstructorProfileRepository
{
    Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InstructorProfile instructorProfile,
        CancellationToken cancellationToken = default);
}
