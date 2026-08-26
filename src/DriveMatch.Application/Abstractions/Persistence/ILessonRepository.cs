using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> HasConflictAsync(
        Guid instructorProfileId,
        DateOnly scheduledDate,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Lesson lesson,
        CancellationToken cancellationToken = default);
}
