using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests.Reject;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Application.Features.LessonRequests;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.LessonRequests.Reject;

public class RejectLessonRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldRejectLessonRequest_WhenRequestExists()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(request.InstructorId, userId)),
            unitOfWork);

        var result = await handler.HandleAsync(
            new RejectLessonRequestCommand(
                request.Id,
                userId));

        Assert.Equal(request.Id, result.LessonRequestId);
        Assert.Equal(LessonRequestStatus.Rejected, result.Status);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestNotFoundException_WhenRequestDoesNotExist()
    {
        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeInstructorProfileRepository(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSaveChanges_WhenRequestDoesNotExist()
    {
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(null),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestNotFoundException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid())));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateDomainException_WhenRequestIsNotPending()
    {
        var request = CreateLessonRequest();
        var userId = Guid.NewGuid();

        request.Accept();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeInstructorProfileRepository(
                CreateInstructorProfile(
                    request.InstructorId,
                    userId)),
            unitOfWork);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(
                    request.Id,
                    userId)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenInstructorDoesNotOwnRequest()
    {
        var request = CreateLessonRequest();
        var authenticatedUserId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();

        var anotherInstructorProfile = CreateInstructorProfile(
            Guid.NewGuid(),
            authenticatedUserId);

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeInstructorProfileRepository(anotherInstructorProfile),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(
                    request.Id,
                    authenticatedUserId)));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowLessonRequestForbiddenException_WhenInstructorProfileDoesNotExist()
    {
        var request = CreateLessonRequest();
        var authenticatedUserId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RejectLessonRequestHandler(
            new FakeLessonRequestRepository(request),
            new FakeInstructorProfileRepository(null),
            unitOfWork);

        await Assert.ThrowsAsync<LessonRequestForbiddenException>(
            () => handler.HandleAsync(
                new RejectLessonRequestCommand(
                    request.Id,
                    authenticatedUserId)));

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
}

