using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class LessonRequestTests
{
    [Fact]
    public void Constructor_ShouldCreatePendingRequest_WhenDataIsValid()
    {
        var studentId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        var request = new LessonRequest(
            Guid.NewGuid(),
            studentId,
            instructorId,
            new DateOnly(2026, 8, 30),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            true,
            " Tenho dificuldade com baliza. ");

        Assert.Equal(studentId, request.StudentId);
        Assert.Equal(instructorId, request.InstructorId);
        Assert.Equal(new DateOnly(2026, 8, 30), request.RequestedDate);
        Assert.Equal(new TimeOnly(14, 0), request.StartTime);
        Assert.Equal(new TimeOnly(15, 0), request.EndTime);
        Assert.True(request.UsesStudentVehicle);
        Assert.Equal("Tenho dificuldade com baliza.", request.StudentMessage);
        Assert.Equal(LessonRequestStatus.Pending, request.Status);
        Assert.NotEqual(default, request.CreatedAt);
        Assert.Null(request.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldNormalizeEmptyMessageToNull()
    {
        var request = CreateRequest(" ");

        Assert.Null(request.StudentMessage);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new LessonRequest(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0),
                false,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenInstructorIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new LessonRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0),
                false,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentAndInstructorAreSame()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<DomainException>(() =>
            new LessonRequest(
                Guid.NewGuid(),
                userId,
                userId,
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0),
                false,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenTimeRangeIsInvalid()
    {
        Assert.Throws<DomainException>(() =>
            new LessonRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(15, 0),
                new TimeOnly(14, 0),
                false,
                null));
    }

    [Fact]
    public void Accept_ShouldChangeStatusFromPendingToAccepted()
    {
        var request = CreateRequest();

        request.Accept();

        Assert.Equal(LessonRequestStatus.Accepted, request.Status);
        Assert.NotNull(request.UpdatedAt);
    }

    [Fact]
    public void Confirm_ShouldChangeStatusFromAcceptedToConfirmed()
    {
        var request = CreateRequest();
        request.Accept();

        request.Confirm();

        Assert.Equal(LessonRequestStatus.Confirmed, request.Status);
        Assert.NotNull(request.UpdatedAt);
    }

    [Fact]
    public void Reject_ShouldChangeStatusFromPendingToRejected()
    {
        var request = CreateRequest();

        request.Reject();

        Assert.Equal(LessonRequestStatus.Rejected, request.Status);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusFromPendingToCancelled()
    {
        var request = CreateRequest();

        request.Cancel();

        Assert.Equal(LessonRequestStatus.Cancelled, request.Status);
    }

    [Fact]
    public void Expire_ShouldChangeStatusFromPendingToExpired()
    {
        var request = CreateRequest();

        request.Expire();

        Assert.Equal(LessonRequestStatus.Expired, request.Status);
    }

    [Fact]
    public void Accept_ShouldThrowDomainException_WhenRequestIsNotPending()
    {
        var request = CreateRequest();
        request.Reject();

        Assert.Throws<DomainException>(() => request.Accept());
    }

    [Fact]
    public void Confirm_ShouldThrowDomainException_WhenRequestIsNotAccepted()
    {
        var request = CreateRequest();

        Assert.Throws<DomainException>(() => request.Confirm());
    }

    [Fact]
    public void Reject_ShouldThrowDomainException_WhenRequestIsNotPending()
    {
        var request = CreateRequest();
        request.Accept();

        Assert.Throws<DomainException>(() => request.Reject());
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenRequestIsNotPending()
    {
        var request = CreateRequest();
        request.Accept();

        Assert.Throws<DomainException>(() => request.Cancel());
    }

    [Fact]
    public void Expire_ShouldThrowDomainException_WhenRequestIsNotPending()
    {
        var request = CreateRequest();
        request.Accept();

        Assert.Throws<DomainException>(() => request.Expire());
    }

    private static LessonRequest CreateRequest(string? message = null)
    {
        return new LessonRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 30),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            false,
            message);
    }
}
