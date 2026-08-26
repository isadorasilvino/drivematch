using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Reviews.Create;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Reviews.Create;

public class CreateReviewHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateReview_WhenLessonIsCompleted()
    {
        var lesson = CreateCompletedLesson();
        var reviewRepository = new FakeReviewRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            reviewRepository,
            unitOfWork);

        var command = new CreateReviewCommand(
            lesson.Id,
            5,
            " Excelente aula. ");

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.ReviewId);
        Assert.Equal(lesson.Id, result.LessonId);
        Assert.Equal(lesson.StudentId, result.StudentId);
        Assert.Equal(lesson.InstructorId, result.InstructorId);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Excelente aula.", result.Comment);
        Assert.NotEqual(default, result.CreatedAt);

        Assert.NotNull(reviewRepository.AddedReview);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var handler = new CreateReviewHandler(
            new FakeLessonRepository(null),
            new FakeReviewRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    Guid.NewGuid(),
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotCompletedException_WhenLessonIsNotCompleted()
    {
        var lesson = CreateLesson();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeReviewRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotCompletedException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReviewAlreadyExistsException_WhenLessonAlreadyHasReview()
    {
        var lesson = CreateCompletedLesson();

        var reviewRepository = new FakeReviewRepository
        {
            ReviewExists = true
        };

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            reviewRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ReviewAlreadyExistsException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenReviewAlreadyExists()
    {
        var lesson = CreateCompletedLesson();

        var reviewRepository = new FakeReviewRepository
        {
            ReviewExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            reviewRepository,
            unitOfWork);

        await Assert.ThrowsAsync<ReviewAlreadyExistsException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    5,
                    null)));

        Assert.Null(reviewRepository.AddedReview);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRatingIsInvalid()
    {
        var lesson = CreateCompletedLesson();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeReviewRepository(),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    6,
                    null)));

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

    private static Lesson CreateCompletedLesson()
    {
        var lesson = CreateLesson();
        lesson.StartCheckIn();
        lesson.ConfirmCheckIn();
        lesson.Complete();

        return lesson;
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

    private sealed class FakeReviewRepository : IReviewRepository
    {
        public bool ReviewExists { get; set; }
        public Review? AddedReview { get; private set; }

        public Task<bool> ExistsForLessonAsync(
            Guid lessonId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ReviewExists);
        }

        public Task AddAsync(
            Review review,
            CancellationToken cancellationToken = default)
        {
            AddedReview = review;
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
