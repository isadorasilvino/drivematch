using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests;

namespace DriveMatch.Application.Features.LessonRequests.Cancel;

public sealed class CancelLessonRequestHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelLessonRequestHandler(
        ILessonRequestRepository lessonRequestRepository,
        IStudentProfileRepository studentProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRequestRepository = lessonRequestRepository;
        _studentProfileRepository = studentProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelLessonRequestResult> HandleAsync(
        CancelLessonRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var lessonRequest =
            await _lessonRequestRepository.GetByIdAsync(
                command.LessonRequestId,
                cancellationToken);

        if (lessonRequest is null)
            throw new LessonRequestNotFoundException(
                command.LessonRequestId);

        var studentProfile =
            await _studentProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (studentProfile is null ||
            studentProfile.Id != lessonRequest.StudentId)
        {
            throw new LessonRequestForbiddenException();
        }

        lessonRequest.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelLessonRequestResult(
            lessonRequest.Id,
            lessonRequest.Status);
    }
}