using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.LessonRequests.GetReceived;

public sealed class GetReceivedLessonRequestsHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;

    public GetReceivedLessonRequestsHandler(
        ILessonRequestRepository lessonRequestRepository)
    {
        _lessonRequestRepository = lessonRequestRepository;
    }

    public async Task<IReadOnlyCollection<LessonRequestListResult>>
        HandleAsync(
            GetReceivedLessonRequestsQuery query,
            CancellationToken cancellationToken = default)
    {
        var requests =
            await _lessonRequestRepository.GetByInstructorUserIdAsync(
                query.UserId,
                cancellationToken);

        return requests
            .Select(request => new LessonRequestListResult(
                request.LessonRequestId,
                request.StudentName,
                request.InstructorName,
                request.RequestedDate,
                request.StartTime,
                request.EndTime,
                request.UsesStudentVehicle,
                request.StudentMessage,
                request.Status,
                request.CreatedAt,
                request.UpdatedAt))
            .ToArray();
    }
}