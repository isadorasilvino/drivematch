using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class LessonRequestRepository
    : ILessonRequestRepository
{
    private readonly DriveMatchDbContext _context;

    public LessonRequestRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<LessonRequest?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.LessonRequests
            .FirstOrDefaultAsync(
                request => request.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        LessonRequest lessonRequest,
        CancellationToken cancellationToken = default)
    {
        await _context.LessonRequests.AddAsync(
            lessonRequest,
            cancellationToken);
    }
}
