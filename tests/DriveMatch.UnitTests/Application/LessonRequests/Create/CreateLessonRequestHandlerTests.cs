using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests.Create;
using DriveMatch.Application.Abstractions.Time;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.LessonRequests.Create;

public class CreateLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreatePendingLessonRequest_WhenDataIsValid()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var lessonRequestRepository = new FakeLessonRequestRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            lessonRequestRepository,
            unitOfWork,
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        var command = new CreateLessonRequestCommand(
            studentProfile.UserId,
            instructorProfile.Id,
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            true,
            " Quero praticar baliza. ");

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.LessonRequestId);
        Assert.Equal(studentProfile.Id, result.StudentId);
        Assert.Equal(instructorProfile.Id, result.InstructorId);
        Assert.Equal(LessonRequestStatus.Pending, result.Status);
        Assert.True(result.UsesStudentVehicle);
        Assert.Equal("Quero praticar baliza.", result.StudentMessage);

        Assert.NotNull(lessonRequestRepository.AddedLessonRequest);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowStudentProfileNotFoundException_WhenStudentDoesNotExist()
    {
        var instructorProfile = CreateActiveInstructorProfile();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(null),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            new FakeLessonRequestRepository(),
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        await Assert.ThrowsAsync<StudentProfileNotFoundException>(
            () => handler.HandleAsync(
                CreateCommand(
                    Guid.NewGuid(),
                    instructorProfile.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileNotFoundException_WhenInstructorDoesNotExist()
    {
        var studentProfile = CreateStudentProfile();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(null),
            new FakeAvailabilityRepository(true),
            new FakeLessonRequestRepository(),
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(
                CreateCommand(
                    studentProfile.UserId,
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorNotActiveException_WhenInstructorIsNotActive()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateInstructorProfile();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            new FakeLessonRequestRepository(),
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        await Assert.ThrowsAsync<InstructorNotActiveException>(
            () => handler.HandleAsync(CreateCommand(
                studentProfile.UserId,
                instructorProfile.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowStudentVehicleNotAcceptedException_WhenInstructorDoesNotAcceptStudentVehicle()
    {
        var studentProfile = CreateStudentProfile();

        var instructorProfile = new InstructorProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "MG",
            new Money(120m),
            true,
            true,
            false);

        instructorProfile.Activate();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            new FakeLessonRequestRepository(),
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        var command = new CreateLessonRequestCommand(
            studentProfile.UserId,
            instructorProfile.Id,
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            true,
            null);

        await Assert.ThrowsAsync<StudentVehicleNotAcceptedException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenInstructorHasNoAvailability()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(false),
            new FakeLessonRequestRepository(),
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(
                CreateCommand(
                    studentProfile.UserId,
                    instructorProfile.Id)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenInstructorIsUnavailable()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var lessonRequestRepository = new FakeLessonRequestRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(false),
            lessonRequestRepository,
            unitOfWork,
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(
                CreateCommand(
                    studentProfile.UserId,
                    instructorProfile.Id)));

        Assert.Null(lessonRequestRepository.AddedLessonRequest);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenRequestedIntervalIsNotAValidSlot()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var lessonRequestRepository =
            new FakeLessonRequestRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            lessonRequestRepository,
            unitOfWork,
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 30, 12, 0, 0)));

        var command = new CreateLessonRequestCommand(
            studentProfile.UserId,
            instructorProfile.Id,
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 10),
            new TimeOnly(15, 10),
            false,
            null);

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(command));

        Assert.Null(
            lessonRequestRepository.AddedLessonRequest);

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenRequestedDateIsInThePast()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var lessonRequestRepository =
            new FakeLessonRequestRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            lessonRequestRepository,
            unitOfWork,
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 1, 12, 0, 0)));

        var command = CreateCommand(
            studentProfile.UserId,
            instructorProfile.Id);

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(command));

        Assert.Null(lessonRequestRepository.AddedLessonRequest);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenRequestedSlotHasAlreadyStartedToday()
    {
        var studentProfile = CreateStudentProfile();
        var instructorProfile = CreateActiveInstructorProfile();

        var lessonRequestRepository =
            new FakeLessonRequestRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateLessonRequestHandler(
            new FakeStudentProfileRepository(studentProfile),
            new FakeInstructorProfileRepository(instructorProfile),
            new FakeAvailabilityRepository(true),
            lessonRequestRepository,
            unitOfWork,
            new FakeDateTimeProvider(
                new DateTime(2026, 8, 31, 14, 30, 0)));

        var command = CreateCommand(
            studentProfile.UserId,
            instructorProfile.Id);

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(command));

        Assert.Null(lessonRequestRepository.AddedLessonRequest);
        Assert.False(unitOfWork.SaveChangesCalled);
    }


    private static CreateLessonRequestCommand CreateCommand(
        Guid userId,
        Guid instructorProfileId)
    {
        return new CreateLessonRequestCommand(
            userId,
            instructorProfileId,
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            false,
            null);
    }

    private static StudentProfile CreateStudentProfile()
    {
        return new StudentProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
    }

    private static InstructorProfile CreateInstructorProfile()
    {
        return new InstructorProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "MG",
            new Money(120m),
            true,
            true,
            true);
    }

    private static InstructorProfile CreateActiveInstructorProfile()
    {
        var profile = CreateInstructorProfile();
        profile.Activate();

        return profile;
    }

    private sealed class FakeStudentProfileRepository
        : IStudentProfileRepository
    {
        private readonly StudentProfile? _profile;

        public FakeStudentProfileRepository(StudentProfile? profile)
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
            return Task.FromResult(_profile?.UserId == userId);
        }

        public Task AddAsync(
            StudentProfile studentProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeInstructorProfileRepository
        : IInstructorProfileRepository
    {
        private readonly InstructorProfile? _profile;

        public FakeInstructorProfileRepository(InstructorProfile? profile)
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
            return Task.FromResult(_profile?.UserId == userId);
        }

        public Task<IReadOnlyCollection<InstructorSearchItem>> SearchAsync(
            string city,
            string state,
            ExperienceLevel experienceLevel,
            bool usesStudentVehicle,
            decimal? maxPricePerLesson,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<InstructorSearchItem>>(
                Array.Empty<InstructorSearchItem>());
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

        public Task<IReadOnlyCollection<Availability>> GetByInstructorProfileIdAsync(
            Guid instructorProfileId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Availability>>(
                Array.Empty<Availability>());
        }

        public Task<IReadOnlyCollection<Availability>> GetActiveByInstructorProfileIdAndDayAsync(
            Guid instructorProfileId,
            DayOfWeek dayOfWeek,
            CancellationToken cancellationToken = default)
        {
            if (!_hasAvailability)
            {
                return Task.FromResult<IReadOnlyCollection<Availability>>(
                    Array.Empty<Availability>());
            }

            IReadOnlyCollection<Availability> availabilities =
            [
                new Availability(
                Guid.NewGuid(),
                instructorProfileId,
                dayOfWeek,
                new TimeOnly(14, 0),
                new TimeOnly(18, 0),
                60,
                0)
            ];

            return Task.FromResult(availabilities);
        }

        public Task<bool> HasActiveAvailabilityAsync(
            Guid instructorProfileId,
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

    private sealed class FakeLessonRequestRepository
        : ILessonRequestRepository
    {
        public LessonRequest? AddedLessonRequest { get; private set; }

        public Task AddAsync(
            LessonRequest lessonRequest,
            CancellationToken cancellationToken = default)
        {
            AddedLessonRequest = lessonRequest;
            return Task.CompletedTask;
        }

        public Task<LessonRequest?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<LessonRequest?>(null);
        }

        public Task<IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>> GetByStudentUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<
                DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>
                result = Array.Empty<
                    DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>> GetByInstructorUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<
                DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>
                result = Array.Empty<
                    DriveMatch.Application.Abstractions.Persistence.Models.LessonRequestListItem>();

            return Task.FromResult(result);
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

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime localNow)
        {
            LocalNow = localNow;
        }

        public DateTime LocalNow { get; }
    }
}
