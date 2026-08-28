using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests;

namespace DriveMatch.Application.Features.LessonRequests.Reject;

public sealed class RejectLessonRequestHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectLessonRequestHandler(
        ILessonRequestRepository lessonRequestRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRequestRepository = lessonRequestRepository;
        _instructorProfileRepository = instructorProfileRepository;
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

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != lessonRequest.InstructorId)
        {
            throw new LessonRequestForbiddenException();
        }

        lessonRequest.Reject();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RejectLessonRequestResult(
            lessonRequest.Id,
            lessonRequest.Status);
    }
}