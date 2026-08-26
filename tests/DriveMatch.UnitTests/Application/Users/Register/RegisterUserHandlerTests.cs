using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Users.Register;

public class RegisterUserHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldRegisterUser_WhenDataIsValid()
    {
        var userRepository = new FakeUserRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new RegisterUserHandler(
            userRepository,
            unitOfWork,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Isadora Silvino",
            "ISADORA@EMAIL.COM",
            "plain-password",
            UserRole.Student);

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal("Isadora Silvino", result.Name);
        Assert.Equal("isadora@email.com", result.Email);

        Assert.NotNull(userRepository.AddedUser);
        Assert.Equal("isadora@email.com", userRepository.AddedUser.Email);
        Assert.Equal("HASHED:plain-password", userRepository.AddedUser.PasswordHash);
        Assert.Equal(UserRole.Student, userRepository.AddedUser.Role);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldNormalizeEmailBeforeCheckingExistence()
    {
        var userRepository = new FakeUserRepository();
        var handler = CreateHandler(userRepository);

        var command = new RegisterUserCommand(
            "Isadora Silvino",
            "  ISADORA@EMAIL.COM  ",
            "password",
            UserRole.Student);

        await handler.HandleAsync(command);

        Assert.Equal(
            "isadora@email.com",
            userRepository.LastCheckedEmail);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowUserAlreadyExistsException_WhenEmailAlreadyExists()
    {
        var userRepository = new FakeUserRepository
        {
            EmailExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new RegisterUserHandler(
            userRepository,
            unitOfWork,
            new FakePasswordHasher());

        var command = new RegisterUserCommand(
            "Isadora Silvino",
            "isadora@email.com",
            "password",
            UserRole.Student);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(
            () => handler.HandleAsync(command));

        Assert.Null(userRepository.AddedUser);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldUsePasswordHasher()
    {
        var passwordHasher = new FakePasswordHasher();
        var handler = new RegisterUserHandler(
            new FakeUserRepository(),
            new FakeUnitOfWork(),
            passwordHasher);

        var command = new RegisterUserCommand(
            "Isadora Silvino",
            "isadora@email.com",
            "my-secret",
            UserRole.Instructor);

        await handler.HandleAsync(command);

        Assert.Equal("my-secret", passwordHasher.LastPassword);
    }

    private static RegisterUserHandler CreateHandler(
        FakeUserRepository userRepository)
    {
        return new RegisterUserHandler(
            userRepository,
            new FakeUnitOfWork(),
            new FakePasswordHasher());
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public bool EmailExists { get; set; }
        public string? LastCheckedEmail { get; private set; }
        public User? AddedUser { get; private set; }

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(null);
        }

        public Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            LastCheckedEmail = email;
            return Task.FromResult(EmailExists);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            AddedUser = user;
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

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string? LastPassword { get; private set; }

        public string Hash(string password)
        {
            LastPassword = password;
            return $"HASHED:{password}";
        }
    }
}
