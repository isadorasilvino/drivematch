using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface ILessonRequestRepository
{
    Task<LessonRequest?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        LessonRequest lessonRequest,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LessonRequestListItem>> GetByStudentUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LessonRequestListItem>> GetByInstructorUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
