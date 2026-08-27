using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DriveMatch.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new InvalidOperationException(
                "O identificador do usuário autenticado não foi encontrado.");

        return userId;
    }
}