using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Users.Account.GetMyAccount;
using DriveMatch.Application.Features.Users.Account;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Users.Account.GetMyAccount;

public class GetMyAccountHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAccount_WhenUserExists()
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            "Isadora Silvino",
            "isadora@email.com",
            "hashed-password",
            UserRole.Student);

        var userRepository = new FakeUserRepository(user);

        var handler = new GetMyAccountHandler(
            userRepository);

        var query = new GetMyAccountQuery(userId);

        var result = await handler.HandleAsync(query);

        Assert.Equal(userId, result.UserId);
        Assert.Equal("Isadora Silvino", result.Name);
        Assert.Equal("isadora@email.com", result.Email);
        Assert.Equal(UserRole.Student, result.Role);

        Assert.Equal(userId, userRepository.LastRequestedUserId);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        var userRepository = new FakeUserRepository(null);

        var handler = new GetMyAccountHandler(
            userRepository);

        var query = new GetMyAccountQuery(userId);

        await Assert.ThrowsAsync<AccountNotFoundException>(
            () => handler.HandleAsync(query));

        Assert.Equal(userId, userRepository.LastRequestedUserId);
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
}