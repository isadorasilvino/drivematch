using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.LessonRequests.GetMine;

public sealed class GetMineLessonRequestsHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;

    public GetMineLessonRequestsHandler(
        ILessonRequestRepository lessonRequestRepository)
    {
        _lessonRequestRepository = lessonRequestRepository;
    }

    public async Task<IReadOnlyCollection<LessonRequestListResult>>
        HandleAsync(
            GetMineLessonRequestsQuery query,
            CancellationToken cancellationToken = default)
    {
        var requests =
            await _lessonRequestRepository.GetByStudentUserIdAsync(
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