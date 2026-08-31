using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class LessonTests
{
    [Fact]
    public void Constructor_ShouldCreateScheduledLesson_WhenDataIsValid()
    {
        var studentId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var lessonRequestId = Guid.NewGuid();

        var lesson = new Lesson(
            Guid.NewGuid(),
            studentId,
            instructorId,
            lessonRequestId,
            new DateOnly(2026, 8, 30),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));

        Assert.Equal(studentId, lesson.StudentId);
        Assert.Equal(instructorId, lesson.InstructorId);
        Assert.Equal(lessonRequestId, lesson.LessonRequestId);
        Assert.Equal(new DateOnly(2026, 8, 30), lesson.ScheduledDate);
        Assert.Equal(new TimeOnly(14, 0), lesson.StartTime);
        Assert.Equal(new TimeOnly(15, 0), lesson.EndTime);
        Assert.Equal(LessonStatus.Scheduled, lesson.Status);
        Assert.Null(lesson.StartedAt);
        Assert.Null(lesson.CheckInAt);
        Assert.Null(lesson.CompletedAt);
        Assert.Null(lesson.CancelledAt);
        Assert.NotEqual(default, lesson.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Lesson(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0)));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenInstructorIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Lesson(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0)));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenLessonRequestIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Lesson(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0)));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentAndInstructorAreSame()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<DomainException>(() =>
            new Lesson(
                Guid.NewGuid(),
                userId,
                userId,
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(14, 0),
                new TimeOnly(15, 0)));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenTimeRangeIsInvalid()
    {
        Assert.Throws<DomainException>(() =>
            new Lesson(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 8, 30),
                new TimeOnly(15, 0),
                new TimeOnly(14, 0)));
    }

    [Fact]
    public void StartCheckIn_ShouldChangeStatusToCheckIn()
    {
        var lesson = CreateLesson();

        var token = lesson.StartCheckIn();

        Assert.Equal(LessonStatus.CheckIn, lesson.Status);
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(token, lesson.CheckInToken);
        Assert.NotNull(lesson.CheckInTokenExpiresAt);
        Assert.True(lesson.CheckInTokenExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void ConfirmCheckIn_ShouldChangeStatusToInProgress()
    {
        var lesson = CreateLesson();
        var token = lesson.StartCheckIn();

        lesson.ConfirmCheckIn(token);

        Assert.Equal(LessonStatus.InProgress, lesson.Status);
        Assert.NotNull(lesson.CheckInAt);
        Assert.NotNull(lesson.StartedAt);
        Assert.Null(lesson.CheckInToken);
        Assert.Null(lesson.CheckInTokenExpiresAt);
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        var lesson = CreateLesson();
        var token = lesson.StartCheckIn();
        lesson.ConfirmCheckIn(token);

        lesson.Complete();

        Assert.Equal(LessonStatus.Completed, lesson.Status);
        Assert.NotNull(lesson.CompletedAt);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var lesson = CreateLesson();

        lesson.Cancel();

        Assert.Equal(LessonStatus.Cancelled, lesson.Status);
        Assert.NotNull(lesson.CancelledAt);
    }

    [Fact]
    public void MarkAsNotAttended_ShouldChangeStatusToNotAttended()
    {
        var lesson = CreateLesson();

        lesson.MarkAsNotAttended();

        Assert.Equal(LessonStatus.NotAttended, lesson.Status);
    }

    [Fact]
    public void StartCheckIn_ShouldThrowDomainException_WhenLessonIsNotScheduled()
    {
        var lesson = CreateLesson();
        lesson.Cancel();

        Assert.Throws<DomainException>(() => lesson.StartCheckIn());
    }

    [Fact]
    public void ConfirmCheckIn_ShouldThrowDomainException_WhenLessonIsNotInCheckIn()
    {
        var lesson = CreateLesson();

        Assert.Throws<DomainException>(() => lesson.ConfirmCheckIn("qualquer-token"));
    }

    [Fact]
    public void Complete_ShouldThrowDomainException_WhenLessonIsNotInProgress()
    {
        var lesson = CreateLesson();

        Assert.Throws<DomainException>(() => lesson.Complete());
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenLessonIsNotScheduled()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();

        Assert.Throws<DomainException>(() => lesson.Cancel());
    }

    [Fact]
    public void MarkAsNotAttended_ShouldThrowDomainException_WhenLessonIsNotScheduled()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();

        Assert.Throws<DomainException>(() => lesson.MarkAsNotAttended());
    }

    [Fact]
    public void CompletedLesson_ShouldNotAllowReturningToPreviousState()
    {
        var lesson = CreateLesson();
        var token = lesson.StartCheckIn();
        lesson.ConfirmCheckIn(token);
        lesson.Complete();

        Assert.Throws<DomainException>(() => lesson.StartCheckIn());
    }

    [Fact]
    public void ConfirmCheckIn_ShouldThrowDomainException_WhenTokenIsInvalid()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();

        Assert.Throws<DomainException>(
            () => lesson.ConfirmCheckIn("token-invalido"));

        Assert.Equal(LessonStatus.CheckIn, lesson.Status);
        Assert.Null(lesson.CheckInAt);
        Assert.Null(lesson.StartedAt);
    }

    [Fact]
    public void ConfirmCheckIn_ShouldThrowDomainException_WhenTokenIsEmpty()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();

        Assert.Throws<DomainException>(
            () => lesson.ConfirmCheckIn(string.Empty));

        Assert.Equal(LessonStatus.CheckIn, lesson.Status);
    }

    [Fact]
    public void StartCheckIn_ShouldGenerateNewToken_WhenCheckInIsAlreadyInProgress()
    {
        var lesson = CreateLesson();

        var firstToken = lesson.StartCheckIn();
        var firstExpiration = lesson.CheckInTokenExpiresAt;

        var secondToken = lesson.StartCheckIn();

        Assert.NotEqual(firstToken, secondToken);
        Assert.Equal(LessonStatus.CheckIn, lesson.Status);
        Assert.Equal(secondToken, lesson.CheckInToken);
        Assert.NotNull(lesson.CheckInTokenExpiresAt);
        Assert.True(lesson.CheckInTokenExpiresAt >= firstExpiration);
    }

    private static Lesson CreateLesson()
    {
        return new Lesson(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 30),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));
    }
}
