using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly DriveMatchDbContext _context;

    public UserRepository(DriveMatchDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .FirstOrDefaultAsync(
                user => user.Id == id,
                cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AnyAsync(
                user => user.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(
            user,
            cancellationToken);
    }
}
