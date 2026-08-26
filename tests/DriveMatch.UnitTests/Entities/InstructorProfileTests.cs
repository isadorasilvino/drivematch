using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Entities;

public class InstructorProfileTests
{
    [Fact]
    public void Constructor_ShouldCreateInstructorProfile_WhenDataIsValid()
    {
        var userId = Guid.NewGuid();
        var price = new Money(100m);

        var profile = new InstructorProfile(
            Guid.NewGuid(),
            userId,
            "Instrutor com experiência em alunos iniciantes.",
            5,
            "Belo Horizonte",
            "mg",
            price,
            true,
            true,
            true);

        Assert.Equal(userId, profile.UserId);
        Assert.Equal("Instrutor com experiência em alunos iniciantes.", profile.Description);
        Assert.Equal(5, profile.ExperienceYears);
        Assert.Equal("Belo Horizonte", profile.City);
        Assert.Equal("MG", profile.State);
        Assert.Equal(price, profile.PricePerLesson);
        Assert.True(profile.AcceptsBeginners);
        Assert.True(profile.AcceptsExperiencedStudents);
        Assert.True(profile.AcceptsStudentVehicle);
        Assert.Equal(InstructorProfileStatus.Draft, profile.Status);
        Assert.NotEqual(default, profile.CreatedAt);
        Assert.Null(profile.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenUserIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new InstructorProfile(
                Guid.NewGuid(),
                Guid.Empty,
                "Descrição",
                5,
                "Belo Horizonte",
                "MG",
                new Money(100m),
                true,
                true,
                true));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenDescriptionIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new InstructorProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                " ",
                5,
                "Belo Horizonte",
                "MG",
                new Money(100m),
                true,
                true,
                true));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenExperienceYearsIsNegative()
    {
        Assert.Throws<DomainException>(() =>
            new InstructorProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Descrição",
                -1,
                "Belo Horizonte",
                "MG",
                new Money(100m),
                true,
                true,
                true));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenPriceIsNull()
    {
        Assert.Throws<DomainException>(() =>
            new InstructorProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Descrição",
                5,
                "Belo Horizonte",
                "MG",
                null!,
                true,
                true,
                true));
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var profile = CreateProfile();

        profile.UpdateDescription("Nova descrição");

        Assert.Equal("Nova descrição", profile.Description);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void UpdateExperienceYears_ShouldUpdateExperienceYears()
    {
        var profile = CreateProfile();

        profile.UpdateExperienceYears(8);

        Assert.Equal(8, profile.ExperienceYears);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void UpdateLocation_ShouldUpdateCityAndState()
    {
        var profile = CreateProfile();

        profile.UpdateLocation("Contagem", "mg");

        Assert.Equal("Contagem", profile.City);
        Assert.Equal("MG", profile.State);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePrice()
    {
        var profile = CreateProfile();
        var newPrice = new Money(120m);

        profile.UpdatePrice(newPrice);

        Assert.Equal(newPrice, profile.PricePerLesson);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void UpdateServicePreferences_ShouldUpdatePreferences()
    {
        var profile = CreateProfile();

        profile.UpdateServicePreferences(
            false,
            true,
            false);

        Assert.False(profile.AcceptsBeginners);
        Assert.True(profile.AcceptsExperiencedStudents);
        Assert.False(profile.AcceptsStudentVehicle);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldChangeStatusToActive()
    {
        var profile = CreateProfile();

        profile.Activate();

        Assert.Equal(InstructorProfileStatus.Active, profile.Status);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        var profile = CreateProfile();

        profile.Deactivate();

        Assert.Equal(InstructorProfileStatus.Inactive, profile.Status);
        Assert.NotNull(profile.UpdatedAt);
    }

    private static InstructorProfile CreateProfile()
    {
        return new InstructorProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Instrutor com experiência.",
            5,
            "Belo Horizonte",
            "MG",
            new Money(100m),
            true,
            true,
            true);
    }
}
