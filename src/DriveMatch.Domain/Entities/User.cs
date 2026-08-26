using DriveMatch.Domain.Common;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User()
    {
        Name = null!;
        Email = null!;
        PasswordHash = null!;
    }

    public User(
        Guid id,
        string name,
        string email,
        string passwordHash,
        UserRole role)
        : base(id)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidatePasswordHash(passwordHash);

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string name, string email)
    {
        ValidateName(name);
        ValidateEmail(email);

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do usuário deve ser informado.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O email do usuário deve ser informado.");
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("O hash da senha deve ser informado.");
    }
}
