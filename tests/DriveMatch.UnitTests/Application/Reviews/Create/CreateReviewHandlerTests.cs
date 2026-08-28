using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Reviews;
using DriveMatch.Application.Features.Reviews.Create;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.Reviews.Create;

public class CreateReviewHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateReview_WhenLessonIsCompletedAndBelongsToAuthenticatedStudent()
    {
        var studentUserId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var studentProfile = CreateStudentProfile(
            studentProfileId,
            studentUserId);

        var lesson = CreateCompletedLesson(studentProfileId);

        var reviewRepository = new FakeReviewRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            reviewRepository,
            unitOfWork);

        var command = new CreateReviewCommand(
            lesson.Id,
            studentUserId,
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
            new FakeStudentProfileRepository(null),
            new FakeReviewRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReviewForbiddenException_WhenStudentProfileDoesNotExist()
    {
        var studentProfileId = Guid.NewGuid();
        var lesson = CreateCompletedLesson(studentProfileId);

        var unitOfWork = new FakeUnitOfWork();
        var reviewRepository = new FakeReviewRepository();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(null),
            reviewRepository,
            unitOfWork);

        await Assert.ThrowsAsync<ReviewForbiddenException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    Guid.NewGuid(),
                    5,
                    null)));

        Assert.Null(reviewRepository.AddedReview);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReviewForbiddenException_WhenLessonBelongsToAnotherStudent()
    {
        var authenticatedUserId = Guid.NewGuid();

        var authenticatedStudentProfile =
            CreateStudentProfile(
                Guid.NewGuid(),
                authenticatedUserId);

        var lesson =
            CreateCompletedLesson(Guid.NewGuid());

        var reviewRepository = new FakeReviewRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(authenticatedStudentProfile),
            reviewRepository,
            unitOfWork);

        await Assert.ThrowsAsync<ReviewForbiddenException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    authenticatedUserId,
                    5,
                    null)));

        Assert.Null(reviewRepository.AddedReview);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotCompletedException_WhenLessonIsNotCompleted()
    {
        var studentUserId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var studentProfile =
            CreateStudentProfile(
                studentProfileId,
                studentUserId);

        var lesson = CreateLesson(studentProfileId);

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            new FakeReviewRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotCompletedException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    studentUserId,
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReviewAlreadyExistsException_WhenLessonAlreadyHasReview()
    {
        var studentUserId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var studentProfile =
            CreateStudentProfile(
                studentProfileId,
                studentUserId);

        var lesson = CreateCompletedLesson(studentProfileId);

        var reviewRepository = new FakeReviewRepository
        {
            ReviewExists = true
        };

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            reviewRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ReviewAlreadyExistsException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    studentUserId,
                    5,
                    null)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenReviewAlreadyExists()
    {
        var studentUserId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var studentProfile =
            CreateStudentProfile(
                studentProfileId,
                studentUserId);

        var lesson = CreateCompletedLesson(studentProfileId);

        var reviewRepository = new FakeReviewRepository
        {
            ReviewExists = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            reviewRepository,
            unitOfWork);

        await Assert.ThrowsAsync<ReviewAlreadyExistsException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    studentUserId,
                    5,
                    null)));

        Assert.Null(reviewRepository.AddedReview);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRatingIsInvalid()
    {
        var studentUserId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();

        var studentProfile =
            CreateStudentProfile(
                studentProfileId,
                studentUserId);

        var lesson = CreateCompletedLesson(studentProfileId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateReviewHandler(
            new FakeLessonRepository(lesson),
            new FakeStudentProfileRepository(studentProfile),
            new FakeReviewRepository(),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CreateReviewCommand(
                    lesson.Id,
                    studentUserId,
                    6,
                    null)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static Lesson CreateLesson(Guid studentProfileId)
    {
        return new Lesson(
            Guid.NewGuid(),
            studentProfileId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));
    }

    private static Lesson CreateCompletedLesson(Guid studentProfileId)
    {
        var lesson = CreateLesson(studentProfileId);

        lesson.StartCheckIn();
        lesson.ConfirmCheckIn();
        lesson.Complete();

        return lesson;
    }

    private static StudentProfile CreateStudentProfile(
        Guid profileId,
        Guid userId)
    {
        return new StudentProfile(
            profileId,
            userId,
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
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

    private sealed class FakeStudentProfileRepository
        : IStudentProfileRepository
    {
        private readonly StudentProfile? _profile;

        public FakeStudentProfileRepository(
            StudentProfile? profile)
        {
            _profile = profile;
        }

        public Task<StudentProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id ? _profile : null);
        }

        public Task<StudentProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId ? _profile : null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId);
        }

        public Task AddAsync(
            StudentProfile studentProfile,
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