using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IReviewRepository
{
    Task<bool> ExistsForLessonAsync(
        Guid lessonId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Review review,
        CancellationToken cancellationToken = default);
}