using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class StudentProfileRepository
    : IStudentProfileRepository
{
    private readonly DriveMatchDbContext _context;

    public StudentProfileRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<StudentProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.StudentProfiles
            .FirstOrDefaultAsync(
                profile => profile.Id == id,
                cancellationToken);
    }

    public Task<StudentProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.StudentProfiles
            .FirstOrDefaultAsync(
                profile => profile.UserId == userId,
                cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.StudentProfiles
            .AnyAsync(
                profile => profile.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        StudentProfile studentProfile,
        CancellationToken cancellationToken = default)
    {
        await _context.StudentProfiles.AddAsync(
            studentProfile,
            cancellationToken);
    }
}
