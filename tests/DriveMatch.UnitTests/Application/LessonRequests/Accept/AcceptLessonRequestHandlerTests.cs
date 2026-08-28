using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.LessonRequests.Accept;

public class AcceptLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldConfirmRequestAndCreateLesson_WhenDataIsValid()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        var lessonRepository = new FakeLessonRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            unitOfWork);

        var result = await handler.HandleAsync(
            new AcceptLessonRequestCommand(
                request.Id,
                userId));

        Assert.Equal(
            LessonRequestStatus.Confirmed,
            result.LessonRequestStatus);

        Assert.Equal(
            LessonStatus.Scheduled,
            result.LessonStatus);

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
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenInstructorDoesNotOwnRequest()
    {
        var request = CreateLessonRequest();
        var authenticatedUserId = Guid.NewGuid();

        var anotherInstructorProfile =
            CreateInstructorProfile(
                Guid.NewGuid(),
                authenticatedUserId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            new FakeLessonRepository(),
            new FakeInstructorProfileRepository(
                anotherInstructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    authenticatedUserId)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenInstructorProfileDoesNotExist()
    {
        var request = CreateLessonRequest();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            new FakeLessonRepository(),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenAvailabilityNoLongerExists()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(false),
            new FakeLessonRepository(),
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    userId)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonScheduleConflictException_WhenInstructorHasConflict()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        var lessonRepository = new FakeLessonRepository
        {
            HasConflict = true
        };

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonScheduleConflictException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    userId)));
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRequestIsNotPending()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        request.Reject();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            new FakeLessonRepository(),
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    userId)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenThereIsConflict()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        var lessonRepository = new FakeLessonRepository
        {
            HasConflict = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeAvailabilityRepository(true),
            lessonRepository,
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            unitOfWork);

        await Assert.ThrowsAsync<LessonScheduleConflictException>(
            () => handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    request.Id,
                    userId)));

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

    private sealed class FakeLessonRequestRepository
        : ILessonRequestRepository
    {
        private readonly LessonRequest? _request;

        public FakeLessonRequestRepository(
            LessonRequest? request)
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
                _profile?.UserId == userId ? _profile : null);
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
                _profile is null
                    ? []
                    : [_profile];

            return Task.FromResult(result);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAvailabilityRepository
        : IAvailabilityRepository
    {
        private readonly bool _hasAvailability;

        public FakeAvailabilityRepository(
            bool hasAvailability)
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

    private sealed class FakeLessonRepository
        : ILessonRepository
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