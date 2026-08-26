using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateActiveUser_WhenDataIsValid()
    {
        var user = new User(
            Guid.NewGuid(),
            "Isadora Silvino",
            "ISADORA@EMAIL.COM",
            "password-hash",
            UserRole.Student);

        Assert.Equal("Isadora Silvino", user.Name);
        Assert.Equal("isadora@email.com", user.Email);
        Assert.Equal("password-hash", user.PasswordHash);
        Assert.Equal(UserRole.Student, user.Role);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldTrimName()
    {
        var user = new User(
            Guid.NewGuid(),
            "  Isadora Silvino  ",
            "isadora@email.com",
            "password-hash",
            UserRole.Student);

        Assert.Equal("Isadora Silvino", user.Name);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenNameIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new User(
                Guid.NewGuid(),
                " ",
                "isadora@email.com",
                "password-hash",
                UserRole.Student));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenEmailIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new User(
                Guid.NewGuid(),
                "Isadora Silvino",
                " ",
                "password-hash",
                UserRole.Student));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenPasswordHashIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new User(
                Guid.NewGuid(),
                "Isadora Silvino",
                "isadora@email.com",
                " ",
                UserRole.Student));
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateNameAndEmail()
    {
        var user = CreateUser();

        user.UpdateProfile(
            "Novo Nome",
            "NOVO@EMAIL.COM");

        Assert.Equal("Novo Nome", user.Name);
        Assert.Equal("novo@email.com", user.Email);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void ChangePasswordHash_ShouldUpdatePasswordHash()
    {
        var user = CreateUser();

        user.ChangePasswordHash("new-password-hash");

        Assert.Equal("new-password-hash", user.PasswordHash);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        var user = CreateUser();

        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldChangeStatusToActive()
    {
        var user = CreateUser();
        user.Deactivate();

        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.NotNull(user.UpdatedAt);
    }

    private static User CreateUser()
    {
        return new User(
            Guid.NewGuid(),
            "Isadora Silvino",
            "isadora@email.com",
            "password-hash",
            UserRole.Student);
    }
}
