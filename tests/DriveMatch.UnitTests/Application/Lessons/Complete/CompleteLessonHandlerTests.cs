using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;
using DriveMatch.Application.Features.Lessons.Complete;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.UnitTests.Application.Lessons;

namespace DriveMatch.UnitTests.Application.Lessons.Complete;

public class CompleteLessonHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCompleteLesson_WhenLessonBelongsToAuthenticatedInstructor()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson = LessonTestHelpers.CreateLesson(instructorProfileId);
        var token = lesson.StartCheckIn();
        lesson.ConfirmCheckIn(token);

        var instructorProfile =
            LessonTestHelpers.CreateInstructorProfile(instructorProfileId, userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new CompleteLessonCommand(lesson.Id, userId));

        Assert.Equal(lesson.Id, result.LessonId);
        Assert.Equal(LessonStatus.Completed, result.Status);
        Assert.NotNull(result.CompletedAt);
        Assert.Equal(LessonStatus.Completed, lesson.Status);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var handler = new CompleteLessonHandler(
            new FakeLessonRepository(null),
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CompleteLessonCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenLessonDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteLessonHandler(
            new FakeLessonRepository(null),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CompleteLessonCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenLessonIsNotInProgress()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(instructorProfileId);

        var instructorProfile =
            LessonTestHelpers.CreateInstructorProfile(instructorProfileId, userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CompleteLessonCommand(lesson.Id, userId)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenLessonBelongsToAnotherInstructor()
    {
        var userId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(Guid.NewGuid());

        var token = lesson.StartCheckIn();
        lesson.ConfirmCheckIn(token);

        var instructorProfile =
            LessonTestHelpers.CreateInstructorProfile(
                Guid.NewGuid(),
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new CompleteLessonCommand(lesson.Id, userId)));

        Assert.Equal(LessonStatus.InProgress, lesson.Status);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeLessonRepository : ILessonRepository
    {
        private readonly Lesson? _lesson;

        public FakeLessonRepository(Lesson? lesson)
        {
            _lesson = lesson;
        }

        public Task<Lesson?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _lesson?.Id == id ? _lesson : null);
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

    private sealed class FakeUnitOfWork : IUnitOfWork
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