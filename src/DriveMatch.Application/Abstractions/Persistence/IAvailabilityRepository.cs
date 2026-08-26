using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Abstractions.Persistence;

public interface IAvailabilityRepository
{
    Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default);
}
