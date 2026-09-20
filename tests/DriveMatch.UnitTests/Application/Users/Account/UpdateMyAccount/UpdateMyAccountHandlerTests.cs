using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Users.Account.UpdateMyAccount;
using DriveMatch.Application.Features.Users.Account;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Users.Account.UpdateMyAccount;

public class UpdateMyAccountHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateAccount_WhenDataIsValid()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Nome Antigo",
            "antigo@email.com",
            "hashed-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateMyAccountHandler(
            userRepository,
            unitOfWork);

        var command = new UpdateMyAccountCommand(
            userId,
            "Isadora Silvino",
            "novo@email.com");

        var result = await handler.HandleAsync(command);

        Assert.Equal(userId, result.UserId);
        Assert.Equal("Isadora Silvino", result.Name);
        Assert.Equal("novo@email.com", result.Email);

        Assert.Equal("Isadora Silvino", user.Name);
        Assert.Equal("novo@email.com", user.Email);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldNormalizeEmail()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora",
            "antigo@email.com",
            "hashed-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateMyAccountHandler(
            userRepository,
            unitOfWork);

        var command = new UpdateMyAccountCommand(
            userId,
            "Isadora",
            "  NOVO@EMAIL.COM  ");

        var result = await handler.HandleAsync(command);

        Assert.Equal(
            "novo@email.com",
            result.Email);

        Assert.Equal(
            "novo@email.com",
            user.Email);

        Assert.Equal(
            "novo@email.com",
            userRepository.LastCheckedEmail);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotCheckEmailExistence_WhenEmailHasNotChanged()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Nome Antigo",
            "isadora@email.com",
            "hashed-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user)
        {
            EmailExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateMyAccountHandler(
            userRepository,
            unitOfWork);

        var command = new UpdateMyAccountCommand(
            userId,
            "Nome Atualizado",
            "  ISADORA@EMAIL.COM  ");

        var result = await handler.HandleAsync(command);

        Assert.Equal(
            "Nome Atualizado",
            result.Name);

        Assert.Equal(
            "isadora@email.com",
            result.Email);

        Assert.False(
            userRepository.ExistsByEmailCalled);

        Assert.True(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAccountEmailAlreadyInUseException_WhenEmailBelongsToAnotherAccount()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora",
            "isadora@email.com",
            "hashed-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user)
        {
            EmailExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateMyAccountHandler(
            userRepository,
            unitOfWork);

        var command = new UpdateMyAccountCommand(
            userId,
            "Isadora Atualizada",
            "outra@conta.com");

        await Assert.ThrowsAsync<AccountEmailAlreadyInUseException>(
            () => handler.HandleAsync(command));

        Assert.Equal(
            "outra@conta.com",
            userRepository.LastCheckedEmail);

        Assert.Equal(
            "Isadora",
            user.Name);

        Assert.Equal(
            "isadora@email.com",
            user.Email);

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        var userRepository =
            new FakeUserRepository(null);

        var unitOfWork =
            new FakeUnitOfWork();

        var handler = new UpdateMyAccountHandler(
            userRepository,
            unitOfWork);

        var command = new UpdateMyAccountCommand(
            userId,
            "Isadora",
            "isadora@email.com");

        await Assert.ThrowsAsync<AccountNotFoundException>(
            () => handler.HandleAsync(command));

        Assert.Equal(
            userId,
            userRepository.LastRequestedUserId);

        Assert.False(
            userRepository.ExistsByEmailCalled);

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public bool EmailExists { get; set; }

        public bool ExistsByEmailCalled { get; private set; }

        public string? LastCheckedEmail { get; private set; }

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
            ExistsByEmailCalled = true;
            LastCheckedEmail = email;

            return Task.FromResult(EmailExists);
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
}