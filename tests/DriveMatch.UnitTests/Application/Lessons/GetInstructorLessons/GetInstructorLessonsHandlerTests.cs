using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.Lessons.GetInstructorLessons;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.Lessons.GetInstructorLessons;

public sealed class GetInstructorLessonsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnInstructorLessons()
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
            InstructorLessons = lessons
        };

        var handler = new GetInstructorLessonsHandler(repository);

        var result = await handler.HandleAsync(
            new GetInstructorLessonsQuery(userId));

        Assert.Same(lessons, result.Lessons);
        Assert.Equal(userId, repository.InstructorUserId);
    }

    private sealed class FakeLessonRepository : ILessonRepository
    {
        public IReadOnlyCollection<LessonListItem> InstructorLessons { get; init; } =
            Array.Empty<LessonListItem>();

        public Guid? InstructorUserId { get; private set; }

        public Task<IReadOnlyCollection<LessonListItem>> GetByStudentUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<LessonListItem>>(
                Array.Empty<LessonListItem>());
        }

        public Task<IReadOnlyCollection<LessonListItem>> GetByInstructorUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            InstructorUserId = userId;
            return Task.FromResult(InstructorLessons);
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