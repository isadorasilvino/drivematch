using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.LessonRequests.Accept;

public class AcceptLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldConfirmRequestAndCreateLesson_WhenDataIsValid()
    {
        var request = CreateLessonRequest();
        var lessonRepository = new FakeLessonRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new AcceptLessonRequestCommand(request.Id));

        Assert.Equal(LessonRequestStatus.Confirmed, result.LessonRequestStatus);
        Assert.Equal(LessonStatus.Scheduled, result.LessonStatus);
        Assert.NotEqual(Guid.Empty, result.LessonId);
        Assert.Equal(request.StudentId, result.StudentId);
        Assert.Equal(request.InstructorId, result.InstructorId);

        Assert.NotNull(lessonRepository.AddedLesson);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestNotFoundException_WhenRequestDoesNotExist()
    {
        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeAvailabilityRepository(true),
            new FakeLessonRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenAvailabilityNoLongerExists()
    {
        var request = CreateLessonRequest();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(false),
            new FakeLessonRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(request.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonScheduleConflictException_WhenInstructorHasConflict()
    {
        var request = CreateLessonRequest();

        var lessonRepository = new FakeLessonRepository
        {
            HasConflict = true
        };

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonScheduleConflictException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(request.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRequestIsNotPending()
    {
        var request = CreateLessonRequest();
        request.Reject();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            new FakeLessonRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(request.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenThereIsConflict()
    {
        var request = CreateLessonRequest();

        var lessonRepository = new FakeLessonRepository
        {
            HasConflict = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            unitOfWork);

        await Assert.ThrowsAsync<LessonScheduleConflictException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(request.Id)));

        Assert.Null(lessonRepository.AddedLesson);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private static LessonRequest CreateLessonRequest()
    {
        return new LessonRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            false,
            null);
    }

    private sealed class FakeLessonRequestRepository
        : ILessonRequestRepository
    {
        private readonly LessonRequest? _request;

        public FakeLessonRequestRepository(LessonRequest? request)
        {
            _request = request;
        }

        public Task<LessonRequest?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _request?.Id == id ? _request : null);
        }

        public Task AddAsync(
            LessonRequest lessonRequest,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAvailabilityRepository
        : IAvailabilityRepository
    {
        private readonly bool _hasAvailability;

        public FakeAvailabilityRepository(bool hasAvailability)
        {
            _hasAvailability = hasAvailability;
        }

        public Task<Availability?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Availability?>(null);
        }

        public Task<bool> HasAvailabilityAsync(
            Guid instructorProfileId,
            DayOfWeek dayOfWeek,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_hasAvailability);
        }

        public Task AddAsync(
            Availability availability,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeLessonRepository : ILessonRepository
    {
        public bool HasConflict { get; set; }
        public Lesson? AddedLesson { get; private set; }

        public Task<Lesson?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Lesson?>(null);
        }

        public Task<bool> HasConflictAsync(
            Guid instructorProfileId,
            DateOnly scheduledDate,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HasConflict);
        }

        public Task AddAsync(
            Lesson lesson,
            CancellationToken cancellationToken = default)
        {
            AddedLesson = lesson;
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
