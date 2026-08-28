using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;
using DriveMatch.Application.Features.Lessons.StartCheckIn;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Lessons.StartCheckIn;

public class StartLessonCheckInHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldStartCheckIn_WhenLessonBelongsToAuthenticatedInstructor()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson = CreateLesson(instructorProfileId);

        var instructorProfile = CreateInstructorProfile(
            instructorProfileId,
            userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new StartLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new StartLessonCheckInCommand(
                lesson.Id,
                userId));

        Assert.Equal(lesson.Id, result.LessonId);
        Assert.Equal(LessonStatus.CheckIn, result.Status);
        Assert.Equal(LessonStatus.CheckIn, lesson.Status);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var handler = new StartLessonCheckInHandler(
            new FakeLessonRepository(null),
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new StartLessonCheckInCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenLessonDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new StartLessonCheckInHandler(
            new FakeLessonRepository(null),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new StartLessonCheckInCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenLessonIsNotScheduled()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson = CreateLesson(instructorProfileId);
        lesson.Cancel();

        var instructorProfile = CreateInstructorProfile(
            instructorProfileId,
            userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new StartLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new StartLessonCheckInCommand(
                    lesson.Id,
                    userId)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenLessonBelongsToAnotherInstructor()
    {
        var authenticatedUserId = Guid.NewGuid();

        var lessonInstructorProfileId = Guid.NewGuid();
        var authenticatedInstructorProfileId = Guid.NewGuid();

        var lesson = CreateLesson(
            lessonInstructorProfileId);

        var authenticatedInstructorProfile =
            CreateInstructorProfile(
                authenticatedInstructorProfileId,
                authenticatedUserId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new StartLessonCheckInHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(
                authenticatedInstructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new StartLessonCheckInCommand(
                    lesson.Id,
                    authenticatedUserId)));

        Assert.Equal(LessonStatus.Scheduled, lesson.Status);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static Lesson CreateLesson(
        Guid instructorProfileId)
    {
        return new Lesson(
            Guid.NewGuid(),
            Guid.NewGuid(),
            instructorProfileId,
            Guid.NewGuid(),
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));
    }

    private static InstructorProfile CreateInstructorProfile(
        Guid profileId,
        Guid userId)
    {
        return new InstructorProfile(
            profileId,
            userId,
            "Instrutor de teste",
            5,
            "Belo Horizonte",
            "MG",
            new Money(80m),
            true,
            true,
            true);
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

    private sealed class FakeInstructorProfileRepository
        : IInstructorProfileRepository
    {
        private readonly InstructorProfile? _profile;

        public FakeInstructorProfileRepository(
            InstructorProfile? profile)
        {
            _profile = profile;
        }

        public Task<InstructorProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id ? _profile : null);
        }

        public Task<InstructorProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId
                    ? _profile
                    : null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.UserId == userId);
        }

        public Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
            string city,
            string state,
            ExperienceLevel experienceLevel,
            bool usesStudentVehicle,
            decimal? maxPricePerLesson,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<InstructorProfile> result =
                Array.Empty<InstructorProfile>();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
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