using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Instructors.UpdateProfile;

public class UpdateInstructorProfileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateInstructorProfile_WhenProfileExists()
    {
        var profile = CreateProfile();
        var repository = new FakeInstructorProfileRepository(profile);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateInstructorProfileHandler(
            repository,
            unitOfWork);

        var command = new UpdateInstructorProfileCommand(
            profile.UserId,
            "Nova descrição profissional.",
            8,
            "Contagem",
            "mg",
            150m,
            false,
            true,
            false);

        var result = await handler.HandleAsync(command);

        Assert.Equal(profile.Id, result.InstructorProfileId);
        Assert.Equal(profile.UserId, result.UserId);
        Assert.Equal("Nova descrição profissional.", result.Description);
        Assert.Equal(8, result.ExperienceYears);
        Assert.Equal("Contagem", result.City);
        Assert.Equal("MG", result.State);
        Assert.Equal(150m, result.PricePerLesson);
        Assert.Equal("BRL", result.Currency);
        Assert.False(result.AcceptsBeginners);
        Assert.True(result.AcceptsExperiencedStudents);
        Assert.False(result.AcceptsStudentVehicle);
        Assert.NotNull(result.UpdatedAt);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var handler = new UpdateInstructorProfileHandler(
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        var command = CreateCommand(Guid.NewGuid());

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenProfileDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateInstructorProfileHandler(
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(
                CreateCommand(Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenPriceIsNegative()
    {
        var profile = CreateProfile();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateInstructorProfileHandler(
            new FakeInstructorProfileRepository(profile),
            unitOfWork);

        var command = new UpdateInstructorProfileCommand(
            profile.UserId,
            "Instrutor",
            5,
            "Belo Horizonte",
            "MG",
            -1m,
            true,
            true,
            true);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static UpdateInstructorProfileCommand CreateCommand(Guid userId)
    {
        return new UpdateInstructorProfileCommand(
            userId,
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "MG",
            120m,
            true,
            true,
            true);
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

        public Task<InstructorProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<InstructorProfile?>(null);
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
