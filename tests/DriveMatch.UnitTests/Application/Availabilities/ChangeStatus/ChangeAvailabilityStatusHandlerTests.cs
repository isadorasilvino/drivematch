using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Domain.Entities;

namespace DriveMatch.UnitTests.Application.Availabilities.ChangeStatus;

public class ChangeAvailabilityStatusHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeactivateAvailability_WhenIsActiveIsFalse()
    {
        var availability = CreateAvailability();
        var repository = new FakeAvailabilityRepository(availability);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            repository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeAvailabilityStatusCommand(
                availability.Id,
                false));

        Assert.False(result.IsActive);
        Assert.False(availability.IsActive);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldActivateAvailability_WhenIsActiveIsTrue()
    {
        var availability = CreateAvailability();
        availability.Deactivate();

        var repository = new FakeAvailabilityRepository(availability);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            repository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeAvailabilityStatusCommand(
                availability.Id,
                true));

        Assert.True(result.IsActive);
        Assert.True(availability.IsActive);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAvailabilityNotFoundException_WhenAvailabilityDoesNotExist()
    {
        var handler = new ChangeAvailabilityStatusHandler(
            new FakeAvailabilityRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    Guid.NewGuid(),
                    false)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenAvailabilityDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            new FakeAvailabilityRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    Guid.NewGuid(),
                    false)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static Availability CreateAvailability()
    {
        return new Availability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));
    }

    private sealed class FakeAvailabilityRepository
        : IAvailabilityRepository
    {
        private readonly Availability? _availability;

        public FakeAvailabilityRepository(
            Availability? availability)
        {
            _availability = availability;
        }

        public Task<Availability?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _availability?.Id == id ? _availability : null);
        }

        public Task AddAsync(
            Availability availability,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
        public Task<bool> HasAvailabilityAsync(
            Guid instructorProfileId,
            DayOfWeek dayOfWeek,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public bool SaveChangesCalled { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;
            return Task.FromResult(1);
        }
    }
}
