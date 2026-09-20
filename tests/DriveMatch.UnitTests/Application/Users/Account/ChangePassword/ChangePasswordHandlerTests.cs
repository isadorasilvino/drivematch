using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Application.Features.Users.Account;
using DriveMatch.Application.Features.Users.Account.ChangePassword;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Users.Account.ChangePassword;

public class ChangePasswordHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldChangePassword_WhenCurrentPasswordIsCorrect()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora Silvino",
            "isadora@email.com",
            "HASHED:current-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user);
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new ChangePasswordHandler(
            userRepository,
            unitOfWork,
            passwordHasher);

        var command = new ChangePasswordCommand(
            userId,
            "current-password",
            "new-password");

        await handler.HandleAsync(command);

        Assert.Equal(
            "HASHED:new-password",
            user.PasswordHash);

        Assert.Equal(
            "current-password",
            passwordHasher.LastVerifiedPassword);

        Assert.Equal(
            "HASHED:current-password",
            passwordHasher.LastVerifiedHash);

        Assert.Equal(
            "new-password",
            passwordHasher.LastHashedPassword);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        var userRepository =
            new FakeUserRepository(null);

        var unitOfWork =
            new FakeUnitOfWork();

        var passwordHasher =
            new FakePasswordHasher();

        var handler = new ChangePasswordHandler(
            userRepository,
            unitOfWork,
            passwordHasher);

        var command = new ChangePasswordCommand(
            userId,
            "current-password",
            "new-password");

        await Assert.ThrowsAsync<AccountNotFoundException>(
            () => handler.HandleAsync(command));

        Assert.Equal(
            userId,
            userRepository.LastRequestedUserId);

        Assert.False(passwordHasher.VerifyCalled);
        Assert.False(passwordHasher.HashCalled);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCurrentPasswordIncorrectException_WhenCurrentPasswordIsIncorrect()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora Silvino",
            "isadora@email.com",
            "HASHED:current-password",
            UserRole.Student);

        var userRepository =
            new FakeUserRepository(user);

        var unitOfWork =
            new FakeUnitOfWork();

        var passwordHasher =
            new FakePasswordHasher();

        var handler = new ChangePasswordHandler(
            userRepository,
            unitOfWork,
            passwordHasher);

        var command = new ChangePasswordCommand(
            userId,
            "wrong-password",
            "new-password");

        await Assert.ThrowsAsync<CurrentPasswordIncorrectException>(
            () => handler.HandleAsync(command));

        Assert.Equal(
            "wrong-password",
            passwordHasher.LastVerifiedPassword);

        Assert.Equal(
            "HASHED:current-password",
            passwordHasher.LastVerifiedHash);

        Assert.False(passwordHasher.HashCalled);

        Assert.Equal(
            "HASHED:current-password",
            user.PasswordHash);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldHashNewPasswordBeforeChangingUserPassword()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora Silvino",
            "isadora@email.com",
            "HASHED:old-password",
            UserRole.Student);

        var passwordHasher =
            new FakePasswordHasher();

        var handler = new ChangePasswordHandler(
            new FakeUserRepository(user),
            new FakeUnitOfWork(),
            passwordHasher);

        var command = new ChangePasswordCommand(
            userId,
            "old-password",
            "my-new-secret");

        await handler.HandleAsync(command);

        Assert.True(passwordHasher.HashCalled);

        Assert.Equal(
            "my-new-secret",
            passwordHasher.LastHashedPassword);

        Assert.Equal(
            "HASHED:my-new-secret",
            user.PasswordHash);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public Guid? LastRequestedUserId { get; private set; }

        public FakeUserRepository(User? user)
        {
            _user = user;
        }

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            LastRequestedUserId = id;

            return Task.FromResult(
                _user?.Id == id
                    ? _user
                    : null);
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
        public bool VerifyCalled { get; private set; }
        public bool HashCalled { get; private set; }

        public string? LastVerifiedPassword { get; private set; }
        public string? LastVerifiedHash { get; private set; }
        public string? LastHashedPassword { get; private set; }

        public string Hash(string password)
        {
            HashCalled = true;
            LastHashedPassword = password;

            return $"HASHED:{password}";
        }

        public bool Verify(
            string password,
            string passwordHash)
        {
            VerifyCalled = true;
            LastVerifiedPassword = password;
            LastVerifiedHash = passwordHash;

            return passwordHash == $"HASHED:{password}";
        }
    }
}