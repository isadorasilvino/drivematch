using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Instructors.CreateProfile;

public class CreateInstructorProfileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateInstructorProfile_WhenDataIsValid()
    {
        var user = CreateInstructorUser();
        var userRepository = new FakeUserRepository(user);
        var instructorProfileRepository = new FakeInstructorProfileRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateInstructorProfileHandler(
            userRepository,
            instructorProfileRepository,
            unitOfWork);

        var command = new CreateInstructorProfileCommand(
            user.Id,
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "mg",
            120m,
            true,
            true,
            true);

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.InstructorProfileId);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("Instrutor experiente.", result.Description);
        Assert.Equal(5, result.ExperienceYears);
        Assert.Equal("Belo Horizonte", result.City);
        Assert.Equal("MG", result.State);
        Assert.Equal(120m, result.PricePerLesson);
        Assert.Equal("BRL", result.Currency);
        Assert.True(result.AcceptsBeginners);
        Assert.True(result.AcceptsExperiencedStudents);
        Assert.True(result.AcceptsStudentVehicle);
        Assert.Equal(InstructorProfileStatus.Draft, result.Status);

        Assert.NotNull(instructorProfileRepository.AddedProfile);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        var handler = new CreateInstructorProfileHandler(
            new FakeUserRepository(null),
            new FakeInstructorProfileRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => handler.HandleAsync(
                CreateCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidUserRoleException_WhenUserIsNotInstructor()
    {
        var user = new User(
            Guid.NewGuid(),
            "Aluno",
            "aluno@email.com",
            "hash",
            UserRole.Student);

        var handler = new CreateInstructorProfileHandler(
            new FakeUserRepository(user),
            new FakeInstructorProfileRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidUserRoleException>(
            () => handler.HandleAsync(
                CreateCommand(user.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileAlreadyExistsException_WhenProfileAlreadyExists()
    {
        var user = CreateInstructorUser();

        var repository = new FakeInstructorProfileRepository
        {
            ProfileExists = true
        };

        var handler = new CreateInstructorProfileHandler(
            new FakeUserRepository(user),
            repository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InstructorProfileAlreadyExistsException>(
            () => handler.HandleAsync(
                CreateCommand(user.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenProfileAlreadyExists()
    {
        var user = CreateInstructorUser();

        var repository = new FakeInstructorProfileRepository
        {
            ProfileExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateInstructorProfileHandler(
            new FakeUserRepository(user),
            repository,
            unitOfWork);

        await Assert.ThrowsAsync<InstructorProfileAlreadyExistsException>(
            () => handler.HandleAsync(
                CreateCommand(user.Id)));

        Assert.Null(repository.AddedProfile);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenPriceIsNegative()
    {
        var user = CreateInstructorUser();

        var handler = new CreateInstructorProfileHandler(
            new FakeUserRepository(user),
            new FakeInstructorProfileRepository(),
            new FakeUnitOfWork());

        var command = new CreateInstructorProfileCommand(
            user.Id,
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
    }

    private static CreateInstructorProfileCommand CreateCommand(Guid userId)
    {
        return new CreateInstructorProfileCommand(
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

    private static User CreateInstructorUser()
    {
        return new User(
            Guid.NewGuid(),
            "Instrutor",
            "instrutor@email.com",
            "hash",
            UserRole.Instructor);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public FakeUserRepository(User? user)
        {
            _user = user;
        }

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _user?.Id == id ? _user : null);
        }

        public Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeInstructorProfileRepository
        : IInstructorProfileRepository
    {
        public bool ProfileExists { get; set; }
        public InstructorProfile? AddedProfile { get; private set; }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ProfileExists);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
            CancellationToken cancellationToken = default)
        {
            AddedProfile = instructorProfile;
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
