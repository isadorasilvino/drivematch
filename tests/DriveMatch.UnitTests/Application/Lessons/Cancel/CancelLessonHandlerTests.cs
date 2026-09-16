using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.Lessons;
using DriveMatch.Application.Features.Lessons.Cancel;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.UnitTests.Application.Lessons;

namespace DriveMatch.UnitTests.Application.Lessons.Cancel;

public class CancelLessonHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCancelLesson_WhenLessonBelongsToAuthenticatedInstructor()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(instructorProfileId);

        var instructorProfile =
            LessonTestHelpers.CreateInstructorProfile(
                instructorProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        var result = await handler.HandleAsync(
            new CancelLessonCommand(
                lesson.Id,
                userId));

        Assert.Equal(
            lesson.Id,
            result.LessonId);

        Assert.Equal(
            LessonStatus.Cancelled,
            result.Status);

        Assert.NotNull(
            result.CancelledAt);

        Assert.Equal(
            LessonStatus.Cancelled,
            lesson.Status);

        Assert.True(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldCancelLesson_WhenLessonBelongsToAuthenticatedStudent()
    {
        var userId = Guid.NewGuid();
        var studentProfileId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                instructorProfileId,
                studentProfileId);

        var studentProfile =
            LessonTestHelpers.CreateStudentProfile(
                studentProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(null),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new CancelLessonCommand(
                lesson.Id,
                userId));

        Assert.Equal(
            lesson.Id,
            result.LessonId);

        Assert.Equal(
            LessonStatus.Cancelled,
            result.Status);

        Assert.NotNull(
            result.CancelledAt);

        Assert.Equal(
            LessonStatus.Cancelled,
            lesson.Status);

        Assert.True(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonNotFoundException_WhenLessonDoesNotExist()
    {
        var handler = new CancelLessonHandler(
            new FakeLessonRepository(null),
            new FakeInstructorProfileRepository(null),
            new FakeStudentProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonNotFoundException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenLessonIsNotScheduled()
    {
        var userId = Guid.NewGuid();
        var instructorProfileId = Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                instructorProfileId);

        lesson.StartCheckIn();

        var instructorProfile =
            LessonTestHelpers.CreateInstructorProfile(
                instructorProfileId,
                userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(
                    lesson.Id,
                    userId)));

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenUserIsNotLessonParticipant()
    {
        var authenticatedUserId =
            Guid.NewGuid();

        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                Guid.NewGuid());

        var anotherInstructorProfile =
            LessonTestHelpers.CreateInstructorProfile(
                Guid.NewGuid(),
                authenticatedUserId);

        var anotherStudentProfile =
            LessonTestHelpers.CreateStudentProfile(
                Guid.NewGuid(),
                authenticatedUserId);

        var unitOfWork =
            new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(
                anotherInstructorProfile),
            new FakeStudentProfileRepository(
                anotherStudentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(
                    lesson.Id,
                    authenticatedUserId)));

        Assert.Equal(
            LessonStatus.Scheduled,
            lesson.Status);

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonForbiddenException_WhenAuthenticatedUserHasNoProfile()
    {
        var lesson =
            LessonTestHelpers.CreateLesson(
                Guid.NewGuid(),
                Guid.NewGuid());

        var unitOfWork =
            new FakeUnitOfWork();

        var handler = new CancelLessonHandler(
            new FakeLessonRepository(lesson),
            new FakeInstructorProfileRepository(null),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonForbiddenException>(
            () => handler.HandleAsync(
                new CancelLessonCommand(
                    lesson.Id,
                    Guid.NewGuid())));

        Assert.Equal(
            LessonStatus.Scheduled,
            lesson.Status);

        Assert.False(
            unitOfWork.SaveChangesCalled);
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

        public Task<IReadOnlyCollection<LessonListItem>>
            GetByStudentUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LessonListItem> result =
                Array.Empty<LessonListItem>();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<LessonListItem>>
            GetByInstructorUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LessonListItem> result =
                Array.Empty<LessonListItem>();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            Lesson lesson,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<LessonScheduleItem>>
            GetBlockingScheduleAsync(
                Guid instructorProfileId,
                DateOnly scheduledDate,
                CancellationToken cancellationToken = default)
        {
            return Task.FromResult<
                IReadOnlyCollection<LessonScheduleItem>>(
                    Array.Empty<LessonScheduleItem>());
        }
    }

    private sealed class FakeUnitOfWork
        : IUnitOfWork
    {
        public bool SaveChangesCalled
        {
            get;
            private set;
        }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;

            return Task.FromResult(1);
        }
    }
}