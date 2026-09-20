using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Users.Account;

namespace DriveMatch.Application.Features.Users.Account.UpdateMyAccount;

public sealed class UpdateMyAccountHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMyAccountHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateMyAccountResult> HandleAsync(
        UpdateMyAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
            throw new AccountNotFoundException(command.UserId);

        var normalizedEmail = command.Email
            .Trim()
            .ToLowerInvariant();

        var emailChanged = !string.Equals(
            user.Email,
            normalizedEmail,
            StringComparison.OrdinalIgnoreCase);

        if (emailChanged)
        {
            var emailAlreadyExists =
                await _userRepository.ExistsByEmailAsync(
                    normalizedEmail,
                    cancellationToken);

            if (emailAlreadyExists)
                throw new AccountEmailAlreadyInUseException(
                    normalizedEmail);
        }

        user.UpdateProfile(
            command.Name,
            normalizedEmail);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new UpdateMyAccountResult(
            user.Id,
            user.Name,
            user.Email);
    }
}