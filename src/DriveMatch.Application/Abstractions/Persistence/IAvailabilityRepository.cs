using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IAvailabilityRepository
{
    Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default);
}
