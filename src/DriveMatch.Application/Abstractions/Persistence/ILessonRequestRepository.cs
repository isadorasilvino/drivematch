using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface ILessonRequestRepository
{
    Task AddAsync(
        LessonRequest lessonRequest,
        CancellationToken cancellationToken = default);
}
