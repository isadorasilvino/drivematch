using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;
using DriveMatch.Application.Features.Lessons.ConfirmCheckIn;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.UnitTests.Application.Lessons;

namespace DriveMatch.UnitTests.Application.Lessons.ConfirmCheckIn;

public class ConfirmLessonCheckInHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldConfirmCheckIn_WhenLessonBelongsToAuthenticatedStudentAndTokenIsValid()
    {
        var userId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson = LessonTestHelpers.CreateLesson(
            instructorProfileId,
            studentProfileId);

        var token = lesson.StartCheckIn();

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                studentProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new ConfirmLessonCheckInCommand(
                lesson.Id,
                userId,
                token));

        Assert.Equal(lesson.Id, result.LessonId);
        Assert.Equal(LessonStatus.InProgress, result.Status);
        Assert.NotNull(result.CheckInAt);
        Assert.NotNull(result.StartedAt);

        Assert.Equal(
            LessonStatus.InProgress,
            lesson.Status);

        Assert.Null(lesson.CheckInToken);
        Assert.Null(lesson.CheckInTokenExpiresAt);

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(null),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "token")));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenStudentProfileDoesNotExist()
    {
        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid());

        var token = lesson.StartCheckIn();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    lesson.Id,
                    Guid.NewGuid(),
                    token)));

        Assert.Equal(
            LessonStatus.CheckIn,
            lesson.Status);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenLessonBelongsToAnotherStudent()
    {
        var userId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                Guid.NewGuid());

        var token = lesson.StartCheckIn();

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                Guid.NewGuid(),
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    lesson.Id,
                    userId,
                    token)));

        Assert.Equal(
            LessonStatus.CheckIn,
            lesson.Status);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenLessonIsNotInCheckIn()
    {
        var userId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                studentProfileId);

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                studentProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    lesson.Id,
                    userId,
                    "token")));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenTokenIsInvalid()
    {
        var userId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                studentProfileId);

        lesson.StartCheckIn();

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                studentProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    lesson.Id,
                    userId,
                    "token-invalido")));

        Assert.Equal(
            LessonStatus.CheckIn,
            lesson.Status);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenTokenIsEmpty()
    {
        var userId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                studentProfileId);

        lesson.StartCheckIn();

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                studentProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConfirmLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new ConfirmLessonCheckInCommand(
                    lesson.Id,
                    userId,
                    string.Empty)));

        Assert.Equal(
            LessonStatus.CheckIn,
            lesson.Status);

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeLessonRepository
        : ILessonRepository
    {
        private readonly Lesson? _lesson;

        public FakeLessonRepository(
            Lesson? lesson)
        {
            _lesson = lesson;
        }

        public Task<Lesson?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _lesson?.Id == id
                    ? _lesson
                    : null);
        }

        public Task<bool> HasConflictAsync(
            Guid instructorProfileId,
            DateOnly scheduledDate,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(
            Lesson lesson,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork
        : IUnitOfWork
    {
        public bool SaveChangesCalled { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;

            return Task.FromResult(1);
        }
    }
}