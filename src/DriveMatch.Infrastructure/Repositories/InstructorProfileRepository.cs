using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class InstructorProfileRepository
    : IInstructorProfileRepository
{
    private readonly DriveMatchDbContext _context;

    public InstructorProfileRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<InstructorProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.InstructorProfiles
            .FirstOrDefaultAsync(
                profile => profile.Id == id,
                cancellationToken);
    }

    public Task<InstructorProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.InstructorProfiles
            .FirstOrDefaultAsync(
                profile => profile.UserId == userId,
                cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.InstructorProfiles
            .AnyAsync(
                profile => profile.UserId == userId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
        string city,
        string state,
        ExperienceLevel experienceLevel,
        bool usesStudentVehicle,
        decimal? maxPricePerLesson,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InstructorProfiles
            .AsNoTracking()
            .Where(profile =>
                profile.Status == InstructorProfileStatus.Active &&
                profile.City == city &&
                profile.State == state);

        query = experienceLevel switch
        {
            ExperienceLevel.Beginner =>
                query.Where(profile => profile.AcceptsBeginners),

            ExperienceLevel.Experienced =>
                query.Where(profile => profile.AcceptsExperiencedStudents),

            _ => query
        };

        if (usesStudentVehicle)
        {
            query = query.Where(
                profile => profile.AcceptsStudentVehicle);
        }

        if (maxPricePerLesson.HasValue)
        {
            query = query.Where(
                profile =>
                    profile.PricePerLesson.Amount <=
                    maxPricePerLesson.Value);
        }

        return await query
            .OrderBy(profile => profile.PricePerLesson.Amount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(
        InstructorProfile instructorProfile,
        CancellationToken cancellationToken = default)
    {
        await _context.InstructorProfiles.AddAsync(
            instructorProfile,
            cancellationToken);
    }
}
