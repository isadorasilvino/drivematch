using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class ReviewRepository : IReviewRepository
{
    private readonly DriveMatchDbContext _context;

    public ReviewRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsForLessonAsync(
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reviews.AnyAsync(
            review => review.LessonId == lessonId,
            cancellationToken);
    }

    public async Task AddAsync(
        Review review,
        CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(
            review,
            cancellationToken);
    }
}
