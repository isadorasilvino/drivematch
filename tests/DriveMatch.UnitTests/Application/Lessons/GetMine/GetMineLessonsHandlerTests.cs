using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.Lessons.GetMine;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Lessons.GetMine;

public sealed class GetMineLessonsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnStudentLessons()
    {
        var userId = Guid.NewGuid();

        IReadOnlyCollection<LessonListItem> lessons =
        [
            new LessonListItem(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Aluno Teste",
                "Instrutor Teste",
                new DateOnly(2026, 9, 9),
                new TimeOnly(10, 0),
                new TimeOnly(10, 45),
                LessonStatus.Scheduled,
                null,
                null,
                null,
                null,
                DateTime.UtcNow)
        ];

        var repository = new FakeLessonRepository
        {
            StudentLessons = lessons
        };

        var handler = new GetMineLessonsHandler(repository);

        var result = await handler.HandleAsync(
            new GetMineLessonsQuery(userId));

        Assert.Same(lessons, result.Lessons);
        Assert.Equal(userId, repository.StudentUserId);
    }

    private sealed class FakeLessonRepository : ILessonRepository
    {
        public IReadOnlyCollection<LessonListItem> StudentLessons { get; init; } =
            Array.Empty<LessonListItem>();

        public Guid? StudentUserId { get; private set; }

        public Task<IReadOnlyCollection<LessonListItem>> GetByStudentUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            StudentUserId = userId;
            return Task.FromResult(StudentLessons);
        }

        public Task<IReadOnlyCollection<LessonListItem>> GetByInstructorUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<LessonListItem>>(
                Array.Empty<LessonListItem>());
        }

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

        public Task AddAsync(
            Lesson lesson,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<LessonScheduleItem>> GetBlockingScheduleAsync(
            Guid instructorProfileId,
            DateOnly scheduledDate,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<LessonScheduleItem>>(
                Array.Empty<LessonScheduleItem>());
        }
    }
}