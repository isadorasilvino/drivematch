using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Students.CreateProfile;

public class CreateStudentProfileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateStudentProfile_WhenDataIsValid()
    {
        var user = CreateStudentUser();
        var userRepository = new FakeUserRepository(user);
        var studentProfileRepository = new FakeStudentProfileRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateStudentProfileHandler(
            userRepository,
            studentProfileRepository,
            unitOfWork);

        var command = new CreateStudentProfileCommand(
            user.Id,
            "Belo Horizonte",
            "mg",
            ExperienceLevel.Beginner,
            true,
            true);

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.StudentProfileId);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("Belo Horizonte", result.City);
        Assert.Equal("MG", result.State);
        Assert.Equal(ExperienceLevel.Beginner, result.ExperienceLevel);
        Assert.True(result.OwnsVehicle);
        Assert.True(result.HasOwnVehicleForLessons);

        Assert.NotNull(studentProfileRepository.AddedProfile);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        var handler = new CreateStudentProfileHandler(
            new FakeUserRepository(null),
            new FakeStudentProfileRepository(),
            new FakeUnitOfWork());

        var command = CreateCommand(Guid.NewGuid());

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidUserRoleException_WhenUserIsNotStudent()
    {
        var user = new User(
            Guid.NewGuid(),
            "Instrutor",
            "instrutor@email.com",
            "hash",
            UserRole.Instructor);

        var handler = new CreateStudentProfileHandler(
            new FakeUserRepository(user),
            new FakeStudentProfileRepository(),
            new FakeUnitOfWork());

        var command = CreateCommand(user.Id);

        await Assert.ThrowsAsync<InvalidUserRoleException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowStudentProfileAlreadyExistsException_WhenProfileAlreadyExists()
    {
        var user = CreateStudentUser();

        var studentProfileRepository = new FakeStudentProfileRepository
        {
            ProfileExists = true
        };

        var handler = new CreateStudentProfileHandler(
            new FakeUserRepository(user),
            studentProfileRepository,
            new FakeUnitOfWork());

        var command = CreateCommand(user.Id);

        await Assert.ThrowsAsync<StudentProfileAlreadyExistsException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenProfileAlreadyExists()
    {
        var user = CreateStudentUser();

        var studentProfileRepository = new FakeStudentProfileRepository
        {
            ProfileExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateStudentProfileHandler(
            new FakeUserRepository(user),
            studentProfileRepository,
            unitOfWork);

        await Assert.ThrowsAsync<StudentProfileAlreadyExistsException>(
            () => handler.HandleAsync(CreateCommand(user.Id)));

        Assert.Null(studentProfileRepository.AddedProfile);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenVehiclePreferencesAreInvalid()
    {
        var user = CreateStudentUser();

        var handler = new CreateStudentProfileHandler(
            new FakeUserRepository(user),
            new FakeStudentProfileRepository(),
            new FakeUnitOfWork());

        var command = new CreateStudentProfileCommand(
            user.Id,
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            true);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));
    }

    private static CreateStudentProfileCommand CreateCommand(Guid userId)
    {
        return new CreateStudentProfileCommand(
            userId,
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
    }

    private static User CreateStudentUser()
    {
        return new User(
            Guid.NewGuid(),
            "Aluno",
            "aluno@email.com",
            "hash",
            UserRole.Student);
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

    private sealed class FakeStudentProfileRepository
    : IStudentProfileRepository
    {
        public bool ProfileExists { get; set; }
        public StudentProfile? AddedProfile { get; private set; }

        public Task<StudentProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<StudentProfile?>(null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ProfileExists);
        }

        public Task AddAsync(
            StudentProfile studentProfile,
            CancellationToken cancellationToken = default)
        {
            AddedProfile = studentProfile;
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
