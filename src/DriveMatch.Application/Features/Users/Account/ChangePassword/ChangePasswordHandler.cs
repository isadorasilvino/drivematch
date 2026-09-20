using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Application.Features.Users.Account;

namespace DriveMatch.Application.Features.Users.Account.ChangePassword;

public sealed class ChangePasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task HandleAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
            throw new AccountNotFoundException(command.UserId);

        var currentPasswordIsValid = _passwordHasher.Verify(
            command.CurrentPassword,
            user.PasswordHash);

        if (!currentPasswordIsValid)
            throw new CurrentPasswordIncorrectException();

        var newPasswordHash = _passwordHasher.Hash(
            command.NewPassword);

        user.ChangePasswordHash(newPasswordHash);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}