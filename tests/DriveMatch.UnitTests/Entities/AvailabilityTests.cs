using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class AvailabilityTests
{
    [Fact]
    public void Constructor_ShouldCreateAvailability_WhenDataIsValid()
    {
        var instructorProfileId = Guid.NewGuid();
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(12, 0);

        var availability = new Availability(
            Guid.NewGuid(),
            instructorProfileId,
            DayOfWeek.Monday,
            startTime,
            endTime,
            60,
            10);

        Assert.Equal(
            instructorProfileId,
            availability.InstructorProfileId);

        Assert.Equal(
            DayOfWeek.Monday,
            availability.DayOfWeek);

        Assert.Equal(
            startTime,
            availability.StartTime);

        Assert.Equal(
            endTime,
            availability.EndTime);

        Assert.Equal(
            60,
            availability.LessonDurationMinutes);

        Assert.Equal(
            10,
            availability.BreakDurationMinutes);

        Assert.True(availability.IsActive);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenInstructorProfileIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.Empty,
                DayOfWeek.Monday,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                60,
                10));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStartTimeEqualsEndTime()
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DayOfWeek.Monday,
                new TimeOnly(8, 0),
                new TimeOnly(8, 0),
                60,
                10));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStartTimeIsAfterEndTime()
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DayOfWeek.Monday,
                new TimeOnly(13, 0),
                new TimeOnly(12, 0),
                60,
                10));
    }

    [Theory]
    [InlineData(30)]
    [InlineData(40)]
    [InlineData(45)]
    [InlineData(50)]
    [InlineData(60)]
    public void Constructor_ShouldAcceptAllowedLessonDurations(
        int lessonDurationMinutes)
    {
        var availability = new Availability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0),
            lessonDurationMinutes,
            10);

        Assert.Equal(
            lessonDurationMinutes,
            availability.LessonDurationMinutes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(30)]
    public void Constructor_ShouldAcceptAllowedBreakDurations(
        int breakDurationMinutes)
    {
        var availability = new Availability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0),
            60,
            breakDurationMinutes);

        Assert.Equal(
            breakDurationMinutes,
            availability.BreakDurationMinutes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(35)]
    [InlineData(55)]
    [InlineData(90)]
    public void Constructor_ShouldThrowDomainException_WhenLessonDurationIsInvalid(
        int lessonDurationMinutes)
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DayOfWeek.Monday,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                lessonDurationMinutes,
                10));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(60)]
    public void Constructor_ShouldThrowDomainException_WhenBreakDurationIsInvalid(
        int breakDurationMinutes)
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DayOfWeek.Monday,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                60,
                breakDurationMinutes));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenAvailabilityDoesNotFitOneLesson()
    {
        Assert.Throws<DomainException>(() =>
            new Availability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DayOfWeek.Monday,
                new TimeOnly(8, 0),
                new TimeOnly(8, 45),
                60,
                10));
    }

    [Fact]
    public void Update_ShouldUpdateAvailability_WhenDataIsValid()
    {
        var availability = CreateAvailability();

        availability.Update(
            DayOfWeek.Tuesday,
            new TimeOnly(14, 0),
            new TimeOnly(18, 0),
            45,
            15);

        Assert.Equal(
            DayOfWeek.Tuesday,
            availability.DayOfWeek);

        Assert.Equal(
            new TimeOnly(14, 0),
            availability.StartTime);

        Assert.Equal(
            new TimeOnly(18, 0),
            availability.EndTime);

        Assert.Equal(
            45,
            availability.LessonDurationMinutes);

        Assert.Equal(
            15,
            availability.BreakDurationMinutes);
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenTimeRangeIsInvalid()
    {
        var availability = CreateAvailability();

        Assert.Throws<DomainException>(() =>
            availability.Update(
                DayOfWeek.Tuesday,
                new TimeOnly(18, 0),
                new TimeOnly(14, 0),
                60,
                10));
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenLessonDurationIsInvalid()
    {
        var availability = CreateAvailability();

        Assert.Throws<DomainException>(() =>
            availability.Update(
                DayOfWeek.Tuesday,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                25,
                10));
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenBreakDurationIsInvalid()
    {
        var availability = CreateAvailability();

        Assert.Throws<DomainException>(() =>
            availability.Update(
                DayOfWeek.Tuesday,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                60,
                25));
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenAvailabilityDoesNotFitOneLesson()
    {
        var availability = CreateAvailability();

        Assert.Throws<DomainException>(() =>
            availability.Update(
                DayOfWeek.Tuesday,
                new TimeOnly(8, 0),
                new TimeOnly(8, 45),
                60,
                10));
    }

    [Fact]
    public void Deactivate_ShouldSetAvailabilityAsInactive()
    {
        var availability = CreateAvailability();

        availability.Deactivate();

        Assert.False(availability.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetAvailabilityAsActive()
    {
        var availability = CreateAvailability();
        availability.Deactivate();

        availability.Activate();

        Assert.True(availability.IsActive);
    }

    private static Availability CreateAvailability()
    {
        return new Availability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0),
            60,
            10);
    }
}