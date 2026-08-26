using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Features.Users.Register;

public sealed class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists =
            await _userRepository.ExistsByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (emailAlreadyExists)
            throw new UserAlreadyExistsException(normalizedEmail);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            Guid.NewGuid(),
            command.Name,
            normalizedEmail,
            passwordHash,
            command.Role);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Name,
            user.Email);
    }
}
