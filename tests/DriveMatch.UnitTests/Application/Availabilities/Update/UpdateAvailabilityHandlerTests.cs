using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities.Update;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Availabilities.Update;

public class UpdateAvailabilityHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateAvailability_WhenAvailabilityExists()
    {
        var availability = CreateAvailability();
        var repository = new FakeAvailabilityRepository(availability);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateAvailabilityHandler(
            repository,
            unitOfWork);

        var command = new UpdateAvailabilityCommand(
            availability.Id,
            DayOfWeek.Tuesday,
            new TimeOnly(14, 0),
            new TimeOnly(18, 0));

        var result = await handler.HandleAsync(command);

        Assert.Equal(availability.Id, result.AvailabilityId);
        Assert.Equal(DayOfWeek.Tuesday, result.DayOfWeek);
        Assert.Equal(new TimeOnly(14, 0), result.StartTime);
        Assert.Equal(new TimeOnly(18, 0), result.EndTime);
        Assert.True(result.IsActive);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAvailabilityNotFoundException_WhenAvailabilityDoesNotExist()
    {
        var handler = new UpdateAvailabilityHandler(
            new FakeAvailabilityRepository(null),
            new FakeUnitOfWork());

        var command = new UpdateAvailabilityCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenAvailabilityDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateAvailabilityHandler(
            new FakeAvailabilityRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(
                new UpdateAvailabilityCommand(
                    Guid.NewGuid(),
                    DayOfWeek.Monday,
                    new TimeOnly(8, 0),
                    new TimeOnly(12, 0))));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenTimeRangeIsInvalid()
    {
        var availability = CreateAvailability();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateAvailabilityHandler(
            new FakeAvailabilityRepository(availability),
            unitOfWork);

        var command = new UpdateAvailabilityCommand(
            availability.Id,
            DayOfWeek.Monday,
            new TimeOnly(12, 0),
            new TimeOnly(8, 0));

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));

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
