using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Users.Account;

namespace DriveMatch.Application.Features.Users.Account.GetMyAccount;

public sealed class GetMyAccountHandler
{
    private readonly IUserRepository _userRepository;

    public GetMyAccountHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetMyAccountResult> HandleAsync(
        GetMyAccountQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            query.UserId,
            cancellationToken);

        if (user is null)
            throw new AccountNotFoundException(query.UserId);

        return new GetMyAccountResult(
            user.Id,
            user.Name,
            user.Email,
            user.Role);
    }
}