using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests.Reject;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.LessonRequests.Reject;

public class RejectLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldRejectLessonRequest_WhenRequestExists()
    {
        var request = CreateLessonRequest();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            unitOfWork);

        var result = await handler.HandleAsync(
            new RejectLessonRequestCommand(request.Id));

        Assert.Equal(request.Id, result.LessonRequestId);
        Assert.Equal(LessonRequestStatus.Rejected, result.Status);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestNotFoundException_WhenRequestDoesNotExist()
    {
        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenRequestDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRequestIsNotPending()
    {
        var request = CreateLessonRequest();
        request.Accept();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(request.Id)));

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
