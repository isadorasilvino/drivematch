using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DriveMatch.Infrastructure.Services;

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        Guid userId,
        string email,
        UserRole role)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "A chave JWT não foi configurada.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "O issuer JWT não foi configurado.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "A audience JWT não foi configurada.");

        var expiresMinutesValue =
            _configuration["Jwt:ExpiresMinutes"];

        var expiresMinutes =
            int.TryParse(expiresMinutesValue, out var parsed)
                ? parsed
                : 60;

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email),

            new Claim(
                ClaimTypes.Role,
                role.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}