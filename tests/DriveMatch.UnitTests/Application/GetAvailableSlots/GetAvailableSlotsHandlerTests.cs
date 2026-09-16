using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.Availabilities.GetAvailableSlots;
using DriveMatch.Application.Abstractions.Time;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Availabilities.GetAvailableSlots;

public class GetAvailableSlotsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var instructorRepository =
            new FakeInstructorProfileRepository(null);

        var availabilityRepository =
            new FakeAvailabilityRepository();

        var lessonRepository =
            new FakeLessonRepository();

        var handler = new GetAvailableSlotsHandler(
            instructorRepository,
            availabilityRepository,
            lessonRepository,
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var instructorProfileId = Guid.NewGuid();

        await Assert.ThrowsAsync<InstructorProfileNotFoundException>(
            () => handler.HandleAsync(
                new GetAvailableSlotsQuery(
                    instructorProfileId,
                    new DateOnly(2026, 9, 7))));

        Assert.False(availabilityRepository.GetActiveByDayCalled);
        Assert.False(lessonRepository.GetBlockingScheduleCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInstructorUnavailableException_WhenProfileIsNotActive()
    {
        var profile = CreateProfile();

        var availabilityRepository =
            new FakeAvailabilityRepository();

        var lessonRepository =
            new FakeLessonRepository();

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            availabilityRepository,
            lessonRepository,
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        await Assert.ThrowsAsync<InstructorUnavailableException>(
            () => handler.HandleAsync(
                new GetAvailableSlotsQuery(
                    profile.Id,
                    new DateOnly(2026, 9, 7))));

        Assert.False(availabilityRepository.GetActiveByDayCalled);
        Assert.False(lessonRepository.GetBlockingScheduleCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenThereAreNoActiveAvailabilitiesForDay()
    {
        var profile = CreateActiveProfile();

        var availabilityRepository =
            new FakeAvailabilityRepository();

        var lessonRepository =
            new FakeLessonRepository();

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            availabilityRepository,
            lessonRepository,
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Empty(result);
        Assert.True(availabilityRepository.GetActiveByDayCalled);
        Assert.False(lessonRepository.GetBlockingScheduleCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldGenerateSlotsFromActiveAvailability()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(14, 0),
            lessonDurationMinutes: 45,
            breakDurationMinutes: 15);

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository([availability]),
            new FakeLessonRepository(),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Collection(
            result,
            slot =>
            {
                Assert.Equal(new TimeOnly(10, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(10, 45), slot.EndTime);
            },
            slot =>
            {
                Assert.Equal(new TimeOnly(11, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(11, 45), slot.EndTime);
            },
            slot =>
            {
                Assert.Equal(new TimeOnly(12, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(12, 45), slot.EndTime);
            },
            slot =>
            {
                Assert.Equal(new TimeOnly(13, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(13, 45), slot.EndTime);
            });
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveSlot_WhenThereIsBlockingLesson()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(14, 0),
            lessonDurationMinutes: 45,
            breakDurationMinutes: 15);

        var blockingSchedule = new[]
        {
            new LessonScheduleItem(
                new TimeOnly(11, 0),
                new TimeOnly(11, 45))
        };

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository([availability]),
            new FakeLessonRepository(blockingSchedule),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Equal(3, result.Count);

        Assert.DoesNotContain(
            result,
            slot =>
                slot.StartTime == new TimeOnly(11, 0) &&
                slot.EndTime == new TimeOnly(11, 45));
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveSlot_WhenLessonOverlapsSlot()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0),
            lessonDurationMinutes: 45,
            breakDurationMinutes: 15);

        var blockingSchedule = new[]
        {
            new LessonScheduleItem(
                new TimeOnly(10, 30),
                new TimeOnly(11, 15))
        };

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository([availability]),
            new FakeLessonRepository(blockingSchedule),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDistinctOrderedSlots_WhenAvailabilitiesOverlap()
    {
        var profile = CreateActiveProfile();

        var firstAvailability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(11, 0),
            new TimeOnly(13, 0),
            lessonDurationMinutes: 60,
            breakDurationMinutes: 0);

        var secondAvailability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0),
            lessonDurationMinutes: 60,
            breakDurationMinutes: 0);

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository(
                [firstAvailability, secondAvailability]),
            new FakeLessonRepository(),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Collection(
            result,
            slot => Assert.Equal(new TimeOnly(10, 0), slot.StartTime),
            slot => Assert.Equal(new TimeOnly(11, 0), slot.StartTime),
            slot => Assert.Equal(new TimeOnly(12, 0), slot.StartTime));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenDateIsInThePast()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(14, 0),
            lessonDurationMinutes: 60,
            breakDurationMinutes: 0);

        var availabilityRepository =
            new FakeAvailabilityRepository([availability]);

        var lessonRepository =
            new FakeLessonRepository();

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            availabilityRepository,
            lessonRepository,
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 8, 12, 0, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Empty(result);
        Assert.False(availabilityRepository.GetActiveByDayCalled);
        Assert.False(lessonRepository.GetBlockingScheduleCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveStartedSlots_WhenDateIsToday()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(14, 0),
            lessonDurationMinutes: 60,
            breakDurationMinutes: 0);

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository([availability]),
            new FakeLessonRepository(),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 7, 11, 30, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Collection(
            result,
            slot =>
            {
                Assert.Equal(new TimeOnly(12, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(13, 0), slot.EndTime);
            },
            slot =>
            {
                Assert.Equal(new TimeOnly(13, 0), slot.StartTime);
                Assert.Equal(new TimeOnly(14, 0), slot.EndTime);
            });
    }

    [Fact]
    public async Task HandleAsync_ShouldKeepAllSlots_WhenDateIsInTheFuture()
    {
        var profile = CreateActiveProfile();

        var availability = CreateAvailability(
            profile.Id,
            DayOfWeek.Monday,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0),
            lessonDurationMinutes: 60,
            breakDurationMinutes: 0);

        var handler = new GetAvailableSlotsHandler(
            new FakeInstructorProfileRepository(profile),
            new FakeAvailabilityRepository([availability]),
            new FakeLessonRepository(),
            new FakeDateTimeProvider(
                new DateTime(2026, 9, 6, 23, 59, 0)));

        var result = await handler.HandleAsync(
            new GetAvailableSlotsQuery(
                profile.Id,
                new DateOnly(2026, 9, 7)));

        Assert.Equal(2, result.Count);
    }

    private static InstructorProfile CreateProfile()
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

    private static InstructorProfile CreateActiveProfile()
    {
        var profile = CreateProfile();
        profile.Activate();

        return profile;
    }

    private static Availability CreateAvailability(
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int lessonDurationMinutes,
        int breakDurationMinutes)
    {
        return new Availability(
            Guid.NewGuid(),
            instructorProfileId,
            dayOfWeek,
            startTime,
            endTime,
            lessonDurationMinutes,
            breakDurationMinutes);
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

        public Task AddAsync(
            InstructorProfile instructorProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
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

        public Task<InstructorProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id
                    ? _profile
                    : null);
        }
    }

    private sealed class FakeAvailabilityRepository
        : IAvailabilityRepository
    {
        private readonly IReadOnlyCollection<Availability> _availabilities;

        public FakeAvailabilityRepository(
            IReadOnlyCollection<Availability>? availabilities = null)
        {
            _availabilities =
                availabilities ?? Array.Empty<Availability>();
        }

        public bool GetActiveByDayCalled { get; private set; }

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

        public Task<bool> HasActiveAvailabilityAsync(
            Guid instructorProfileId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(
            Availability availability,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Availability>> GetActiveByInstructorProfileIdAndDayAsync(
            Guid instructorProfileId,
            DayOfWeek dayOfWeek,
            CancellationToken cancellationToken = default)
        {
            GetActiveByDayCalled = true;

            return Task.FromResult(
                _availabilities);
        }
    }

    private sealed class FakeLessonRepository
        : ILessonRepository
    {
        private readonly IReadOnlyCollection<LessonScheduleItem> _blockingSchedule;

        public FakeLessonRepository(
            IReadOnlyCollection<LessonScheduleItem>? blockingSchedule = null)
        {
            _blockingSchedule =
                blockingSchedule ?? Array.Empty<LessonScheduleItem>();
        }

        public bool GetBlockingScheduleCalled { get; private set; }

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
            return Task.FromResult(false);
        }

        public Task<IReadOnlyCollection<LessonScheduleItem>> GetBlockingScheduleAsync(
            Guid instructorProfileId,
            DateOnly scheduledDate,
            CancellationToken cancellationToken = default)
        {
            GetBlockingScheduleCalled = true;

            return Task.FromResult(
                _blockingSchedule);
        }


        public Task<IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem>> GetByStudentUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem> result =
                Array.Empty<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem>();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem>> GetByInstructorUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem> result =
                Array.Empty<DriveMatch.Application.Abstractions.Persistence.Models.LessonListItem>();

            return Task.FromResult(result);
        }
        public Task AddAsync(
            Lesson lesson,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
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
