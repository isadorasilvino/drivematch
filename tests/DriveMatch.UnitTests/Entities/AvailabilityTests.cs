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
            endTime);

        Assert.Equal(instructorProfileId, availability.InstructorProfileId);
        Assert.Equal(DayOfWeek.Monday, availability.DayOfWeek);
        Assert.Equal(startTime, availability.StartTime);
        Assert.Equal(endTime, availability.EndTime);
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
                new TimeOnly(12, 0)));
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
                new TimeOnly(8, 0)));
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
                new TimeOnly(12, 0)));
    }

    [Fact]
    public void Update_ShouldUpdateAvailability_WhenDataIsValid()
    {
        var availability = CreateAvailability();

        availability.Update(
            DayOfWeek.Tuesday,
            new TimeOnly(14, 0),
            new TimeOnly(18, 0));

        Assert.Equal(DayOfWeek.Tuesday, availability.DayOfWeek);
        Assert.Equal(new TimeOnly(14, 0), availability.StartTime);
        Assert.Equal(new TimeOnly(18, 0), availability.EndTime);
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenTimeRangeIsInvalid()
    {
        var availability = CreateAvailability();

        Assert.Throws<DomainException>(() =>
            availability.Update(
                DayOfWeek.Tuesday,
                new TimeOnly(18, 0),
                new TimeOnly(14, 0)));
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
            new TimeOnly(12, 0));
    }
}
