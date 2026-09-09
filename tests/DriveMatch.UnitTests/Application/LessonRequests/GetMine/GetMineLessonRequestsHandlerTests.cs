using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.LessonRequests.GetMine;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.UnitTests.Application.LessonRequests.GetMine;

public sealed class GetMineLessonRequestsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnStudentLessonRequests()
    {
        var userId = Guid.NewGuid();
        var lessonRequestId = Guid.NewGuid();

        var repository = new FakeLessonRequestRepository
        {
            StudentRequests =
            [
                new LessonRequestListItem(
                    lessonRequestId,
                    "Aluno Teste",
                    "Instrutor Teste",
                    new DateOnly(2026, 9, 10),
                    new TimeOnly(10, 0),
                    new TimeOnly(11, 0),
                    true,
                    "Quero praticar baliza.",
                    LessonRequestStatus.Pending,
                    new DateTime(2026, 9, 7, 18, 0, 0, DateTimeKind.Utc),
                    null)
            ]
        };

        var handler = new GetMineLessonRequestsHandler(repository);

        var result = await handler.HandleAsync(
            new GetMineLessonRequestsQuery(userId));

        var item = Assert.Single(result);

        Assert.Equal(lessonRequestId, item.LessonRequestId);
        Assert.Equal("Aluno Teste", item.StudentName);
        Assert.Equal("Instrutor Teste", item.InstructorName);
        Assert.Equal(new DateOnly(2026, 9, 10), item.RequestedDate);
        Assert.Equal(new TimeOnly(10, 0), item.StartTime);
        Assert.Equal(new TimeOnly(11, 0), item.EndTime);
        Assert.True(item.UsesStudentVehicle);
        Assert.Equal("Quero praticar baliza.", item.StudentMessage);
        Assert.Equal(LessonRequestStatus.Pending, item.Status);
        Assert.Equal(userId, repository.LastStudentUserId);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenStudentHasNoRequests()
    {
        var repository = new FakeLessonRequestRepository();

        var handler = new GetMineLessonRequestsHandler(repository);

        var result = await handler.HandleAsync(
            new GetMineLessonRequestsQuery(Guid.NewGuid()));

        Assert.Empty(result);
    }

    private sealed class FakeLessonRequestRepository
        : ILessonRequestRepository
    {
        public IReadOnlyCollection<LessonRequestListItem> StudentRequests
        {
            get;
            init;
        } = Array.Empty<LessonRequestListItem>();

        public Guid? LastStudentUserId { get; private set; }

        public Task<LessonRequest?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<LessonRequest?>(null);
        }

        public Task<IReadOnlyCollection<LessonRequestListItem>>
            GetByStudentUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            LastStudentUserId = userId;

            return Task.FromResult(StudentRequests);
        }

        public Task<IReadOnlyCollection<LessonRequestListItem>>
            GetByInstructorUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LessonRequestListItem> result =
                Array.Empty<LessonRequestListItem>();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            LessonRequest lessonRequest,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}