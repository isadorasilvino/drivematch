using DriveMatch.Domain.Entities;
using DriveMatch.Application.Abstractions.Persistence.Models;

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

    Task<IReadOnlyCollection<LessonScheduleItem>> GetBlockingScheduleAsync(
        Guid instructorProfileId,
        DateOnly scheduledDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LessonListItem>> GetByStudentUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LessonListItem>> GetByInstructorUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
