using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Availabilities.Create;

public class CreateAvailabilityHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateAvailability_WhenDataIsValid()
    {
        var instructorProfile = CreateInstructorProfile();
        var instructorProfileRepository =
            new FakeInstructorProfileRepository(instructorProfile);

        var availabilityRepository =
            new FakeAvailabilityRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateAvailabilityHandler(
            instructorProfileRepository,
            availabilityRepository,
            unitOfWork);

        var command = new CreateAvailabilityCommand(
            instructorProfile.UserId,
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.AvailabilityId);
        Assert.Equal(instructorProfile.Id, result.InstructorProfileId);
        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
        Assert.Equal(new TimeOnly(8, 0), result.StartTime);
        Assert.Equal(new TimeOnly(12, 0), result.EndTime);

        Assert.NotNull(availabilityRepository.AddedAvailability);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var handler = new CreateAvailabilityHandler(
            new FakeInstructorProfileRepository(null),
            new FakeAvailabilityRepository(),
            new FakeUnitOfWork());

        var command = new CreateAvailabilityCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenProfileDoesNotExist()
    {
        var availabilityRepository =
            new FakeAvailabilityRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateAvailabilityHandler(
            new FakeInstructorProfileRepository(null),
            availabilityRepository,
            unitOfWork);

        var command = new CreateAvailabilityCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(command));

        Assert.Null(availabilityRepository.AddedAvailability);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenTimeRangeIsInvalid()
    {
        var instructorProfile = CreateInstructorProfile();

        var handler = new CreateAvailabilityHandler(
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(),
            new FakeUnitOfWork());

        var command = new CreateAvailabilityCommand(
            instructorProfile.UserId,
            DayOfWeek.Monday,
            new TimeOnly(12, 0),
            new TimeOnly(8, 0));

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));
    }

    private static InstructorProfile CreateInstructorProfile()
    {
        return new InstructorProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "MG",
            new Money(120m),
            true,
            true,
            true);
    }

    private sealed class FakeInstructorProfileRepository
        : IInstructorProfileRepository
    {
        private readonly InstructorProfile? _profile;

        public FakeInstructorProfileRepository(
            InstructorProfile? profile)
        {
            _profile = profile;
        }

        public Task<InstructorProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId ? _profile : null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_profile?.UserId == userId);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
            string city,
            string state,
            ExperienceLevel experienceLevel,
            bool usesStudentVehicle,
            decimal? maxPricePerLesson,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<InstructorProfile>>(
                Array.Empty<InstructorProfile>());
        }
    }

    private sealed class FakeAvailabilityRepository
    : IAvailabilityRepository
    {
        public Availability? AddedAvailability { get; private set; }

        public Task<Availability?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Availability?>(null);
        }

        public Task AddAsync(
            Availability availability,
            CancellationToken cancellationToken = default)
        {
            AddedAvailability = availability;
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
