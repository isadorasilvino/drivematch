using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Instructors.ChangeStatus;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Instructors.ChangeStatus;

public class ChangeInstructorProfileStatusHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldActivateProfile_WhenThereIsActiveAvailability()
    {
        var profile = CreateProfile();
        var instructorRepository =
            new FakeInstructorProfileRepository(profile);

        var availabilityRepository =
            new FakeAvailabilityRepository(hasActiveAvailability: true);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeInstructorProfileStatusHandler(
            instructorRepository,
            availabilityRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeInstructorProfileStatusCommand(
                profile.UserId,
                true));

        Assert.Equal(profile.Id, result.InstructorProfileId);
        Assert.Equal(InstructorProfileStatus.Active, result.Status);
        Assert.Equal(InstructorProfileStatus.Active, profile.Status);

        Assert.True(
            availabilityRepository.HasActiveAvailabilityCalled);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenTryingToActivateWithoutActiveAvailability()
    {
        var profile = CreateProfile();

        var availabilityRepository =
            new FakeAvailabilityRepository(
                hasActiveAvailability: false);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeInstructorProfileStatusHandler(
            new FakeInstructorProfileRepository(profile),
            availabilityRepository,
            unitOfWork);

        await Assert.ThrowsAsync<InstructorProfileCannotBeActivatedException>(
            () => handler.HandleAsync(
                new ChangeInstructorProfileStatusCommand(
                    profile.UserId,
                    true)));

        Assert.Equal(InstructorProfileStatus.Draft, profile.Status);

        Assert.True(
            availabilityRepository.HasActiveAvailabilityCalled);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeactivateProfile_WithoutRequiringActiveAvailability()
    {
        var profile = CreateProfile();
        profile.Activate();

        var availabilityRepository =
            new FakeAvailabilityRepository(
                hasActiveAvailability: false);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeInstructorProfileStatusHandler(
            new FakeInstructorProfileRepository(profile),
            availabilityRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new ChangeInstructorProfileStatusCommand(
                profile.UserId,
                false));

        Assert.Equal(
            InstructorProfileStatus.Inactive,
            result.Status);

        Assert.Equal(
            InstructorProfileStatus.Inactive,
            profile.Status);

        Assert.False(
            availabilityRepository.HasActiveAvailabilityCalled);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var availabilityRepository =
            new FakeAvailabilityRepository(
                hasActiveAvailability: true);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeInstructorProfileStatusHandler(
            new FakeInstructorProfileRepository(null),
            availabilityRepository,
            unitOfWork);

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(
                new ChangeInstructorProfileStatusCommand(
                    Guid.NewGuid(),
                    true)));

        Assert.False(
            availabilityRepository.HasActiveAvailabilityCalled);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static InstructorProfile CreateProfile()
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
                _profile?.UserId == userId
                    ? _profile
                    : null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId);
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

        public Task<InstructorProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id
                    ? _profile
                    : null);
        }
    }

    private sealed class FakeAvailabilityRepository
        : IAvailabilityRepository
    {
        private readonly bool _hasActiveAvailability;

        public FakeAvailabilityRepository(
            bool hasActiveAvailability)
        {
            _hasActiveAvailability = hasActiveAvailability;
        }

        public bool HasActiveAvailabilityCalled { get; private set; }

        public Task<Availability?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Availability?>(null);
        }

        public Task<IReadOnlyCollection<Availability>> GetByInstructorProfileIdAsync(
            Guid instructorProfileId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Availability>>(
                Array.Empty<Availability>());
        }

        public Task<bool> HasActiveAvailabilityAsync(
            Guid instructorProfileId,
            CancellationToken cancellationToken = default)
        {
            HasActiveAvailabilityCalled = true;

            return Task.FromResult(
                _hasActiveAvailability);
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