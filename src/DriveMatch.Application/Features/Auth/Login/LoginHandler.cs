using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Auth.Login;

public sealed class LoginHandler
{
    private readonly IUserAuthenticationRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(
    IUserAuthenticationRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        var passwordIsValid = _passwordHasher.Verify(
            command.Password,
            user.PasswordHash);

        var token = _tokenService.GenerateToken(
            user.Id,
            user.Email,
            user.Role);

        if (!passwordIsValid)
            throw new InvalidCredentialsException();

        if (user.Status != UserStatus.Active)
            throw new UserInactiveException();

        return new LoginResult(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            token);
    }
}
