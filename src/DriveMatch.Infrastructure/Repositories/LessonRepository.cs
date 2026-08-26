using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class LessonRepository : ILessonRepository
{
    private readonly DriveMatchDbContext _context;

    public LessonRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<Lesson?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Lessons
            .FirstOrDefaultAsync(
                lesson => lesson.Id == id,
                cancellationToken);
    }

    public Task<bool> HasConflictAsync(
        Guid instructorProfileId,
        DateOnly scheduledDate,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default)
    {
        return _context.Lessons.AnyAsync(
            lesson =>
                lesson.InstructorId == instructorProfileId &&
                lesson.ScheduledDate == scheduledDate &&
                lesson.Status != LessonStatus.Cancelled &&
                lesson.Status != LessonStatus.NotAttended &&
                startTime < lesson.EndTime &&
                endTime > lesson.StartTime,
            cancellationToken);
    }

    public async Task AddAsync(
        Lesson lesson,
        CancellationToken cancellationToken = default)
    {
        await _context.Lessons.AddAsync(
            lesson,
            cancellationToken);
    }
}
