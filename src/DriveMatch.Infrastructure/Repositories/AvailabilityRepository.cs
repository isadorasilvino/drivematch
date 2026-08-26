using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class AvailabilityRepository
    : IAvailabilityRepository
{
    private readonly DriveMatchDbContext _context;

    public AvailabilityRepository(
        DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Availabilities
            .FirstOrDefaultAsync(
                availability => availability.Id == id,
                cancellationToken);
    }

    public Task<bool> HasAvailabilityAsync(
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default)
    {
        return _context.Availabilities.AnyAsync(
            availability =>
                availability.InstructorProfileId == instructorProfileId &&
                availability.DayOfWeek == dayOfWeek &&
                availability.IsActive &&
                availability.StartTime <= startTime &&
                availability.EndTime >= endTime,
            cancellationToken);
    }

    public async Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default)
    {
        await _context.Availabilities.AddAsync(
            availability,
            cancellationToken);
    }
}
