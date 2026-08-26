using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons.Cancel;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Lessons.Cancel;

public class CancelLessonHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCancelLesson_WhenLessonIsScheduled()
    {
        var lesson = CreateLesson();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            unitOfWork);

        var result = await handler.HandleAsync(
            new CancelLessonCommand(lesson.Id));

        Assert.Equal(lesson.Id, result.LessonId);
        Assert.Equal(LessonStatus.Cancelled, result.Status);
        Assert.NotNull(result.CancelledAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var handler = new CancelLessonHandler(
            new FakeLessonRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenLessonIsNotScheduled()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(lesson.Id)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static Lesson CreateLesson()
    {
        return new Lesson(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));
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
