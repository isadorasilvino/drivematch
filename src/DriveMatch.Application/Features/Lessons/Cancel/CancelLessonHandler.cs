using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;

namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed class CancelLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelLessonHandler(
        ILessonRepository lessonRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelLessonResult> HandleAsync(
        CancelLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != lesson.InstructorId)
        {
            throw new LessonForbiddenException();
        }

        lesson.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelLessonResult(
            lesson.Id,
            lesson.Status,
            lesson.CancelledAt);
    }
}