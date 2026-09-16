using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
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

    public async Task<IReadOnlyCollection<LessonListItem>> GetByStudentUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from lesson in _context.Lessons.AsNoTracking()
            join studentProfile in _context.StudentProfiles.AsNoTracking()
                on lesson.StudentId equals studentProfile.Id
            join studentUser in _context.Users.AsNoTracking()
                on studentProfile.UserId equals studentUser.Id
            join instructorProfile in _context.InstructorProfiles.AsNoTracking()
                on lesson.InstructorId equals instructorProfile.Id
            join instructorUser in _context.Users.AsNoTracking()
                on instructorProfile.UserId equals instructorUser.Id
            where studentProfile.UserId == userId
            orderby lesson.ScheduledDate descending,
                lesson.StartTime descending,
                lesson.CreatedAt descending
            select new LessonListItem(
                lesson.Id,
                lesson.LessonRequestId,
                studentUser.Name,
                instructorUser.Name,
                lesson.ScheduledDate,
                lesson.StartTime,
                lesson.EndTime,
                lesson.Status,
                lesson.StartedAt,
                lesson.CheckInAt,
                lesson.CompletedAt,
                lesson.CancelledAt,
                lesson.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LessonListItem>> GetByInstructorUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from lesson in _context.Lessons.AsNoTracking()
            join studentProfile in _context.StudentProfiles.AsNoTracking()
                on lesson.StudentId equals studentProfile.Id
            join studentUser in _context.Users.AsNoTracking()
                on studentProfile.UserId equals studentUser.Id
            join instructorProfile in _context.InstructorProfiles.AsNoTracking()
                on lesson.InstructorId equals instructorProfile.Id
            join instructorUser in _context.Users.AsNoTracking()
                on instructorProfile.UserId equals instructorUser.Id
            where instructorProfile.UserId == userId
            orderby lesson.ScheduledDate descending,
                lesson.StartTime descending,
                lesson.CreatedAt descending
            select new LessonListItem(
                lesson.Id,
                lesson.LessonRequestId,
                studentUser.Name,
                instructorUser.Name,
                lesson.ScheduledDate,
                lesson.StartTime,
                lesson.EndTime,
                lesson.Status,
                lesson.StartedAt,
                lesson.CheckInAt,
                lesson.CompletedAt,
                lesson.CancelledAt,
                lesson.CreatedAt))
            .ToListAsync(cancellationToken);
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

    public async Task<IReadOnlyCollection<LessonScheduleItem>> GetBlockingScheduleAsync(
        Guid instructorProfileId,
        DateOnly scheduledDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Where(lesson =>
                lesson.InstructorId == instructorProfileId &&
                lesson.ScheduledDate == scheduledDate &&
                lesson.Status != LessonStatus.Cancelled &&
                lesson.Status != LessonStatus.NotAttended)
            .OrderBy(lesson => lesson.StartTime)
            .Select(lesson => new LessonScheduleItem(
                lesson.StartTime,
                lesson.EndTime))
            .ToArrayAsync(cancellationToken);
    }
}