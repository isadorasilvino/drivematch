using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.LessonRequests.Reject;

public sealed class RejectLessonRequestHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectLessonRequestHandler(
        ILessonRequestRepository lessonRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRequestRepository = lessonRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RejectLessonRequestResult> HandleAsync(
        RejectLessonRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var lessonRequest = await _lessonRequestRepository.GetByIdAsync(
            command.LessonRequestId,
            cancellationToken);

        if (lessonRequest is null)
            throw new LessonRequestNotFoundException(command.LessonRequestId);

        lessonRequest.Reject();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RejectLessonRequestResult(
            lessonRequest.Id,
            lessonRequest.Status);
    }
}
