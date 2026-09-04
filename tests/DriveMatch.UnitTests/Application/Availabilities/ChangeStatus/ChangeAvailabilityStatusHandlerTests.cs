using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities;
using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Availabilities.ChangeStatus;

public class ChangeAvailabilityStatusHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeactivateAvailability_WhenAvailabilityBelongsToAuthenticatedInstructor()
    {
        var instructorUserId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var instructorProfile = CreateInstructorProfile(
            instructorProfileId,
            instructorUserId);

        var availability = CreateAvailability(instructorProfileId);

        var repository = new FakeAvailabilityRepository(availability);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            repository,
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeAvailabilityStatusCommand(
                availability.Id,
                instructorUserId,
                false));

        Assert.False(result.IsActive);
        Assert.False(availability.IsActive);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldActivateAvailability_WhenAvailabilityBelongsToAuthenticatedInstructor()
    {
        var instructorUserId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var instructorProfile = CreateInstructorProfile(
            instructorProfileId,
            instructorUserId);

        var availability = CreateAvailability(instructorProfileId);
        availability.Deactivate();

        var repository = new FakeAvailabilityRepository(availability);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            repository,
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeAvailabilityStatusCommand(
                availability.Id,
                instructorUserId,
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
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    false)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenAvailabilityDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            new FakeAvailabilityRepository(null),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<AvailabilityNotFoundException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    false)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAvailabilityForbiddenException_WhenAvailabilityBelongsToAnotherInstructor()
    {
        var authenticatedUserId = Guid.NewGuid();

        var authenticatedInstructor = CreateInstructorProfile(
            Guid.NewGuid(),
            authenticatedUserId);

        var availability = CreateAvailability(Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            new FakeAvailabilityRepository(availability),
            new FakeInstructorProfileRepository(authenticatedInstructor),
            unitOfWork);

        await Assert.ThrowsAsync<AvailabilityForbiddenException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    availability.Id,
                    authenticatedUserId,
                    false)));

        Assert.True(availability.IsActive);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAvailabilityForbiddenException_WhenInstructorProfileDoesNotExist()
    {
        var availability = CreateAvailability(Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeAvailabilityStatusHandler(
            new FakeAvailabilityRepository(availability),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<AvailabilityForbiddenException>(
            () => handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    availability.Id,
                    Guid.NewGuid(),
                    false)));

        Assert.True(availability.IsActive);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static Availability CreateAvailability(Guid instructorProfileId)
    {
        return new Availability(
            Guid.NewGuid(),
            instructorProfileId,
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0));
    }

    private static InstructorProfile CreateInstructorProfile(
        Guid profileId,
        Guid userId)
    {
        return new InstructorProfile(
            profileId,
            userId,
            "Instrutor de teste",
            5,
            "Belo Horizonte",
            "MG",
            new Money(80m),
            true,
            true,
            true);
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

        public Task<IReadOnlyCollection<Availability>> GetByInstructorProfileIdAsync(
            Guid instructorProfileId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Availability>>(
                Array.Empty<Availability>());
        }
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

        public Task<InstructorProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id ? _profile : null);
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
            return Task.FromResult(
                _profile?.UserId == userId);
        }

        public Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
            string city,
            string state,
            ExperienceLevel experienceLevel,
            bool usesStudentVehicle,
            decimal? maxPricePerLesson,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<InstructorProfile> result =
                Array.Empty<InstructorProfile>();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
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