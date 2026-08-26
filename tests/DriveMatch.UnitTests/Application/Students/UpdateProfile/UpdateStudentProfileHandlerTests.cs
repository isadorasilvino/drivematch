using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Students.UpdateProfile;

public class UpdateStudentProfileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateStudentProfile_WhenProfileExists()
    {
        var profile = CreateProfile();
        var repository = new FakeStudentProfileRepository(profile);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateStudentProfileHandler(
            repository,
            unitOfWork);

        var command = new UpdateStudentProfileCommand(
            profile.UserId,
            "Contagem",
            "mg",
            ExperienceLevel.Experienced,
            true,
            true);

        var result = await handler.HandleAsync(command);

        Assert.Equal(profile.Id, result.StudentProfileId);
        Assert.Equal(profile.UserId, result.UserId);
        Assert.Equal("Contagem", result.City);
        Assert.Equal("MG", result.State);
        Assert.Equal(ExperienceLevel.Experienced, result.ExperienceLevel);
        Assert.True(result.OwnsVehicle);
        Assert.True(result.HasOwnVehicleForLessons);
        Assert.NotNull(result.UpdatedAt);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowStudentProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var handler = new UpdateStudentProfileHandler(
            new FakeStudentProfileRepository(null),
            new FakeUnitOfWork());

        var command = new UpdateStudentProfileCommand(
            Guid.NewGuid(),
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);

        await Assert.ThrowsAsync<StudentProfileNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenProfileDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateStudentProfileHandler(
            new FakeStudentProfileRepository(null),
            unitOfWork);

        var command = new UpdateStudentProfileCommand(
            Guid.NewGuid(),
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);

        await Assert.ThrowsAsync<StudentProfileNotFoundException>(
            () => handler.HandleAsync(command));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenVehiclePreferencesAreInvalid()
    {
        var profile = CreateProfile();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateStudentProfileHandler(
            new FakeStudentProfileRepository(profile),
            unitOfWork);

        var command = new UpdateStudentProfileCommand(
            profile.UserId,
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            true);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static StudentProfile CreateProfile()
    {
        return new StudentProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
    }

    private sealed class FakeStudentProfileRepository
        : IStudentProfileRepository
    {
        private readonly StudentProfile? _profile;

        public FakeStudentProfileRepository(StudentProfile? profile)
        {
            _profile = profile;
        }

        public Task<StudentProfile?> GetByUserIdAsync(
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
            StudentProfile studentProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<StudentProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<StudentProfile?>(null);
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
