using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
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

    public async Task<IReadOnlyCollection<LessonRequestListItem>> GetByStudentUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from request in _context.LessonRequests.AsNoTracking()
            join studentProfile in _context.StudentProfiles.AsNoTracking()
                on request.StudentId equals studentProfile.Id
            join studentUser in _context.Users.AsNoTracking()
                on studentProfile.UserId equals studentUser.Id
            join instructorProfile in _context.InstructorProfiles.AsNoTracking()
                on request.InstructorId equals instructorProfile.Id
            join instructorUser in _context.Users.AsNoTracking()
                on instructorProfile.UserId equals instructorUser.Id
            where studentProfile.UserId == userId
            orderby request.RequestedDate descending,
                request.StartTime descending,
                request.CreatedAt descending
            select new LessonRequestListItem(
                request.Id,
                studentUser.Name,
                instructorUser.Name,
                request.RequestedDate,
                request.StartTime,
                request.EndTime,
                request.UsesStudentVehicle,
                request.StudentMessage,
                request.Status,
                request.CreatedAt,
                request.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LessonRequestListItem>>GetByInstructorUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from request in _context.LessonRequests.AsNoTracking()
            join studentProfile in _context.StudentProfiles.AsNoTracking()
                on request.StudentId equals studentProfile.Id
            join studentUser in _context.Users.AsNoTracking()
                on studentProfile.UserId equals studentUser.Id
            join instructorProfile in _context.InstructorProfiles.AsNoTracking()
                on request.InstructorId equals instructorProfile.Id
            join instructorUser in _context.Users.AsNoTracking()
                on instructorProfile.UserId equals instructorUser.Id
            where instructorProfile.UserId == userId
            orderby request.RequestedDate descending,
                request.StartTime descending,
                request.CreatedAt descending
            select new LessonRequestListItem(
                request.Id,
                studentUser.Name,
                instructorUser.Name,
                request.RequestedDate,
                request.StartTime,
                request.EndTime,
                request.UsesStudentVehicle,
                request.StudentMessage,
                request.Status,
                request.CreatedAt,
                request.UpdatedAt))
            .ToListAsync(cancellationToken);
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
