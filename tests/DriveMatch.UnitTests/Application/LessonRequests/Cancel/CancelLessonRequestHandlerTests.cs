using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Persistence.Models;
using DriveMatch.Application.Features.LessonRequests;
using DriveMatch.Application.Features.LessonRequests.Cancel;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Application.LessonRequests.Cancel;

public class CancelLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCancelLessonRequest_WhenRequestBelongsToAuthenticatedStudent()
    {
        var studentProfileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateLessonRequest(
            studentProfileId);

        var studentProfile = CreateStudentProfile(
            studentProfileId,
            userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        var result = await handler.HandleAsync(
            new CancelLessonRequestCommand(
                request.Id,
                userId));

        Assert.Equal(
            request.Id,
            result.LessonRequestId);

        Assert.Equal(
            LessonRequestStatus.Cancelled,
            result.Status);

        Assert.Equal(
            LessonRequestStatus.Cancelled,
            request.Status);

        Assert.True(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestNotFoundException_WhenRequestDoesNotExist()
    {
        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeStudentProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new CancelLessonRequestCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenRequestDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new CancelLessonRequestCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRequestIsNotPending()
    {
        var studentProfileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateLessonRequest(
            studentProfileId);

        request.Accept();

        var studentProfile = CreateStudentProfile(
            studentProfileId,
            userId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeStudentProfileRepository(studentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new CancelLessonRequestCommand(
                    request.Id,
                    userId)));

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenStudentDoesNotOwnRequest()
    {
        var request = CreateLessonRequest(
            Guid.NewGuid());

        var authenticatedUserId =
            Guid.NewGuid();

        var anotherStudentProfile =
            CreateStudentProfile(
                Guid.NewGuid(),
                authenticatedUserId);

        var unitOfWork =
            new FakeUnitOfWork();

        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeStudentProfileRepository(
                anotherStudentProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new CancelLessonRequestCommand(
                    request.Id,
                    authenticatedUserId)));

        Assert.Equal(
            LessonRequestStatus.Pending,
            request.Status);

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenStudentProfileDoesNotExist()
    {
        var request = CreateLessonRequest(
            Guid.NewGuid());

        var authenticatedUserId =
            Guid.NewGuid();

        var unitOfWork =
            new FakeUnitOfWork();

        var handler = new CancelLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeStudentProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new CancelLessonRequestCommand(
                    request.Id,
                    authenticatedUserId)));

        Assert.False(
            unitOfWork.SaveChangesCalled);
    }

    private static LessonRequest CreateLessonRequest(
        Guid studentProfileId)
    {
        return new LessonRequest(
            Guid.NewGuid(),
            studentProfileId,
            Guid.NewGuid(),
            new DateOnly(2026, 9, 15),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0),
            false,
            null);
    }

    private static StudentProfile CreateStudentProfile(
        Guid profileId,
        Guid userId)
    {
        return new StudentProfile(
            profileId,
            userId,
            "Belo Horizonte",
            "MG",
            ExperienceLevel.Beginner,
            false,
            false);
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
                _request?.Id == id
                    ? _request
                    : null);
        }

        public Task AddAsync(
            LessonRequest lessonRequest,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<LessonRequestListItem>>
            GetByStudentUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LessonRequestListItem> result =
                Array.Empty<LessonRequestListItem>();

            return Task.FromResult(result);
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
    }

    private sealed class FakeStudentProfileRepository
        : IStudentProfileRepository
    {
        private readonly StudentProfile? _profile;

        public FakeStudentProfileRepository(
            StudentProfile? profile)
        {
            _profile = profile;
        }

        public Task<StudentProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _profile?.Id == id
                    ? _profile
                    : null);
        }

        public Task<StudentProfile?> GetByUserIdAsync(
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
            StudentProfile studentProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
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