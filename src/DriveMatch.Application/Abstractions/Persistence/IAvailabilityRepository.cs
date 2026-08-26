using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IAvailabilityRepository
{
    Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> HasAvailabilityAsync(
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default);
}
