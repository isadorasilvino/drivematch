using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IAvailabilityRepository
{
    Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Availability>> GetByInstructorProfileIdAsync(
        Guid instructorProfileId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveAvailabilityAsync(
        Guid instructorProfileId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Availability>> GetActiveByInstructorProfileIdAndDayAsync(
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default);
}
