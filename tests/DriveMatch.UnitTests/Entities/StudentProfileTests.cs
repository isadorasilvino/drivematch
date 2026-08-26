using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class StudentProfileTests
{
    [Fact]
    public void Constructor_ShouldCreateStudentProfile_WhenDataIsValid()
    {
        var userId = Guid.NewGuid();

        var profile = new StudentProfile(
            Guid.NewGuid(),
            userId,
            "Belo Horizonte",
            "mg",
            ExperienceLevel.Beginner,
            true,
            true);

        Assert.Equal(userId, profile.UserId);
        Assert.Equal("Belo Horizonte", profile.City);
        Assert.Equal("MG", profile.State);
        Assert.Equal(ExperienceLevel.Beginner, profile.ExperienceLevel);
        Assert.True(profile.OwnsVehicle);
        Assert.True(profile.HasOwnVehicleForLessons);
        Assert.NotEqual(default, profile.CreatedAt);
        Assert.Null(profile.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenUserIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new StudentProfile(
                Guid.NewGuid(),
                Guid.Empty,
                "Belo Horizonte",
                "MG",
                ExperienceLevel.Beginner,
                false,
                false));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenCityIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new StudentProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                " ",
                "MG",
                ExperienceLevel.Beginner,
                false,
                false));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStateIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new StudentProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Belo Horizonte",
                " ",
                ExperienceLevel.Beginner,
                false,
                false));
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
    public void UpdateExperienceLevel_ShouldUpdateExperienceLevel()
    {
        var profile = CreateProfile();

        profile.UpdateExperienceLevel(ExperienceLevel.Experienced);

        Assert.Equal(ExperienceLevel.Experienced, profile.ExperienceLevel);
        Assert.NotNull(profile.UpdatedAt);
    }

    [Fact]
    public void UpdateVehiclePreferences_ShouldUpdateVehicleInformation()
    {
        var profile = CreateProfile();

        profile.UpdateVehiclePreferences(true, true);

        Assert.True(profile.OwnsVehicle);
        Assert.True(profile.HasOwnVehicleForLessons);
        Assert.NotNull(profile.UpdatedAt);
    }

    private static StudentProfile CreateProfile()
    {
        return new StudentProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
    }
}
